using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class Player : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public Transform hand;
    public SendChoice buttonprefab;
    public CardDisplay carddisplay;
    [HideInInspector] public TMP_Text choicetext;
    [HideInInspector] public Transform choicehand;
    PlayerButton playerbutton;

    public Pawn pawn;
    public int position;
    public Photon.Realtime.Player photonrealtime;

    [HideInInspector] public string choice;
    [HideInInspector] public Card chosencard;
    [HideInInspector] public TileData chosentile;
    enum TypeOfCard { None, Any, Paths, Explorers };

    SendChoice endturnbutton;
    public bool druid = false;
    public bool enchanted = false;
    public bool waiting = false;
    bool turnon = false;
    public bool hermit = false;
    bool didnothing;

    public int plays;
    public int moves;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetParent(GameObject.Find("Store Players").transform);
        this.transform.localPosition = new Vector2(-50, -105);

        hand = this.transform.GetChild(0).transform;
        hand.transform.localPosition = new Vector2(0, -280);
        hand.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        this.transform.GetChild(1).transform.localPosition = new Vector3(0, -520, 0);

        pv = GetComponent<PhotonView>();
        this.name = pv.Owner.NickName;

        if (pv.IsMine)
        {
            choicetext = GameObject.Find("Ability Collector Text").GetComponent<TMP_Text>();
            choicetext.transform.parent.gameObject.SetActive(false);
            choicetext.transform.parent.SetParent(this.transform);
            choicehand = choicetext.transform.parent.GetChild(2);

            GameObject.Find("Flip Path").GetComponent<Button>().onClick.AddListener(FlipButton);
            endturnbutton = GameObject.Find("Do Nothing").GetComponent<SendChoice>();
            endturnbutton.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public IEnumerator ClearGrid()
    {
        List<GameObject> list = new List<GameObject>();
        while (Manager.instance.cardsplayedgrid.childCount > 0)
        {
            yield return new WaitForSeconds(0.05f);
            list.Add(Manager.instance.cardsplayedgrid.GetChild(0).gameObject);
            Manager.instance.cardsplayedgrid.GetChild(0).SetParent(null);
        }

        for (int i = 0; i < list.Count; i++)
            Destroy(list[i]);
    }

    [PunRPC]
    public void CreateExplorerGrid(int cardID)
    {
        Explorer newexplorer = PhotonView.Find(cardID).GetComponent<Explorer>();
        CardDisplay cd = Instantiate(carddisplay, Manager.instance.cardsplayedgrid.transform);
        cd.CardArt(newexplorer);
    }

    [PunRPC]
    public void CreatePathGrid(int cardID, bool flipped)
    {
        Path newpath = PhotonView.Find(cardID).GetComponent<Path>();
        newpath.FlipCard(flipped);
        CardDisplay cd = Instantiate(carddisplay, Manager.instance.cardsplayedgrid.transform);
        cd.CardArt(newpath);
    }

    [PunRPC]
    public void StartPawn(string buttonname, int pawnID, int startingposition)
    {
        pawn = PhotonView.Find(pawnID).GetComponent<Pawn>();
        pawn.SetStart(this, startingposition);
        playerbutton = GameObject.Find(buttonname).GetComponent<PlayerButton>();
        playerbutton.MoveBar();
    }

    public void FlipButton()
    {
        for (int i = 0; i<hand.childCount; i++)
        {
            Path nextcard = hand.GetChild(i).GetComponent<Path>();
            if (nextcard != null)
                nextcard.FlipCard();
        }
    }

    public SendChoice CreateButton(string x)
    {
        SendChoice y = Instantiate(buttonprefab, choicehand);
        y.textbox.text = x;
        y.name = x;
        y.EnableButton(this);
        return y;
    }

    public void AddPlays(int n)
    {
        plays += n;
        photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
        photonView.RPC("UpdateMyText", RpcTarget.All, hand.childCount);
    }

    public void AddMoves(int n)
    {
        moves += n;
        photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
        photonView.RPC("UpdateMyText", RpcTarget.All, hand.childCount);
    }

    [PunRPC]
    void UpdateNumbers(int plays, int moves)
    {
        Manager.instance.numbers.text = $"Plays: {plays} \nMoves: {moves}";
    }

    [PunRPC]
    void UpdateMyText(int cardsinhand)
    {
        playerbutton.textbox.text = $"{this.name}: {cardsinhand} Cards";
    }

    public void Update()
    {
        if (endturnbutton != null)
        {
            if (didnothing)
                endturnbutton.EnableButton(this);
            else
                endturnbutton.DisableButton();
        }
    }

    public IEnumerator TakeTurnRPC(Photon.Realtime.Player requestingplayer)
    {
        photonView.RPC("TakeTurn", requestingplayer);
        turnon = true;
        while (turnon)
            yield return null;
    }

    [PunRPC]
    void TurnDone()
    {
        turnon = false;
    }

    [PunRPC]
    void WaitDone()
    {
        waiting = false;
    }

    [PunRPC]
    IEnumerator WaitForPlayer(string player)
    {
        waiting = true;
        Manager.instance.instructions.text = $"Waiting for {player}...";
        while (waiting)
        {
            yield return null;
        }
    }

    [PunRPC]
    public IEnumerator TakeTurn()
    {
        if (pv.IsMine)
        {
            photonView.RPC("WaitForPlayer", RpcTarget.Others, this.name);
            didnothing = true;
            bool continueturn = true;

            plays = (enchanted) ? 1 : 2;
            moves = (enchanted) ? 1 : 2;
            photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);

            while (plays > 0 && continueturn)
            {
                photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
                photonView.RPC("WaitForPlayer", RpcTarget.Others, this.name);
                Manager.instance.instructions.text = $"Your Turn";

                yield return AskExplorer();
                if (choice == "Do Nothing")
                {
                    didnothing = true;
                    endturnbutton.DisableButton();
                    continueturn = false;
                    break;
                }
                else if (choice == "End Phase")
                {
                    break;
                }
                else
                {
                    didnothing = false;
                    endturnbutton.DisableButton();
                }
            }

            while (plays > 0 && continueturn)
            {
                photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
                Manager.instance.instructions.text = $"Your Turn";
                photonView.RPC("WaitForPlayer", RpcTarget.Others, this.name);

                yield return AskPath();
                if (choice == "Do Nothing")
                {
                    didnothing = true;
                    endturnbutton.DisableButton();
                    continueturn = false;
                    break;
                }
                else if (choice == "End Phase")
                {
                    break;
                }
                else
                {
                    AddPlays(-1);
                    didnothing = false;
                    endturnbutton.DisableButton();
                }
            }

            while (moves > 0 && continueturn)
            {
                photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
                Manager.instance.instructions.text = $"Your Turn";
                photonView.RPC("WaitForPlayer", RpcTarget.Others, this.name);

                yield return AskMove();
                if (choice == "Do Nothing")
                {
                    didnothing = true;
                    endturnbutton.DisableButton();
                    continueturn = false;
                    break;
                }
                else if (choice == "No")
                {
                    break;
                }
                else
                {
                    didnothing = false;
                    AddMoves(-1);
                    endturnbutton.DisableButton();
                }
            }

            if (didnothing)
                DrawCardRPC(2);
            else
                DrawCardRPC(1);

            plays = 0;
            moves = 0;
            enchanted = false;
            hermit = false;
            druid = false;
            didnothing = false;
            photonView.RPC("TurnDone", RpcTarget.All);
            photonView.RPC("ClearGrid", RpcTarget.All);
            photonView.RPC("WaitDone", RpcTarget.All);
        }
        yield return null;
    }

    public IEnumerator AskExplorer()
    {
        if (pv.IsMine)
        {
            bool explorerinhand = false;
            for (int i = 0; i < hand.childCount; i++)
            {
                SendChoice ct = hand.GetChild(i).GetComponent<SendChoice>();
                if (ct.card.explorer)
                {
                    ct.EnableButton(this);
                    explorerinhand = true;
                }
            }

            if (explorerinhand)
            {
                choicetext.transform.parent.gameObject.SetActive(true);
                choicetext.text = $"{this.name}: Play an explorer?";
                SendChoice x = CreateButton("End Phase");

                choice = "";
                chosencard = null;
                while (choice == "")
                    yield return null;

                Destroy(x.gameObject);
                for (int i = 0; i < hand.childCount; i++)
                    hand.GetChild(i).GetComponent<SendChoice>().DisableButton();
                choicetext.transform.parent.gameObject.SetActive(false);

                if (chosencard != null)
                {
                    didnothing = false;
                    Card playedcard = chosencard;

                    photonView.RPC("CreateExplorerGrid", RpcTarget.All, playedcard.pv.ViewID);
                    photonView.RPC("SendDiscard", RpcTarget.All, playedcard.pv.ViewID);
                    AddPlays(-1);
                    yield return playedcard.GetComponent<Explorer>().PlayThis(this);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
                choice = "End Phase";
        }
    }

    IEnumerator AskMove()
    {
        if (pv.IsMine)
        {
            TileData nextposition = pawn.CanMove();
            if (nextposition != null)
            {
                nextposition.choicescript.EnableButton(this);
                choicetext.transform.parent.gameObject.SetActive(true);
                choicetext.text = $"{this.name}: Move forwards?";
                choice = "";
                chosencard = null;

                List<SendChoice> listofchoices = new List<SendChoice>();
                listofchoices.Add(CreateButton("Yes"));
                listofchoices.Add(CreateButton("No"));

                while (choice == "")
                    yield return null;
                for (int i = 0; i < listofchoices.Count; i++)
                    Destroy(listofchoices[i].gameObject);
                choicetext.transform.parent.gameObject.SetActive(false);
                nextposition.choicescript.DisableButton();

                if (choice == "Yes")
                {
                    pawn.MoveOn(nextposition, druid);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
                choice = "End Phase";
        }
    }

    public IEnumerator AskPath()
    {
        if (pv.IsMine)
        {
            bool PathInHand = false;
            for (int i = 0; i < hand.childCount; i++)
            {
                SendChoice ct = hand.GetChild(i).GetComponent<SendChoice>();
                if (ct.card.path)
                {
                    ct.EnableButton(this);
                    PathInHand = true;
                }
            }

            if (PathInHand)
            {
                choicetext.transform.parent.gameObject.SetActive(true);
                choicetext.text = $"{this.name}: Play a path?";
                SendChoice x = CreateButton("End Phase");

                choice = "";
                chosencard = null;
                while (choice == "")
                    yield return null;

                Destroy(x.gameObject);
                for (int i = 0; i < hand.childCount; i++)
                    hand.GetChild(i).GetComponent<SendChoice>().DisableButton();
                choicetext.transform.parent.gameObject.SetActive(false);

                if (chosencard != null)
                {
                    choicetext.transform.parent.gameObject.SetActive(true);
                    choicetext.text = $"{this.name}: Place that Path in the forest";

                    Path cardforlater = chosencard.GetComponent<Path>();
                    for (int i = 0; i < Manager.instance.listoftiles.Count; i++)
                    {
                        TileData nexttile = Manager.instance.listoftiles[i];
                        if (!nexttile.river)
                            nexttile.choicescript.EnableButton(this);
                    }

                    choice = "";
                    chosentile = null;
                    while (choice == "")
                        yield return null;

                    for (int i = 0; i < Manager.instance.listoftiles.Count; i++)
                        Manager.instance.listoftiles[i].choicescript.DisableButton();
                    choicetext.transform.parent.gameObject.SetActive(false);

                    photonView.RPC("PathToForest", RpcTarget.All, cardforlater.pv.ViewID, chosentile.position, cardforlater.flipped);
                    photonView.RPC("CreatePathGrid", RpcTarget.All, cardforlater.pv.ViewID, cardforlater.flipped);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
                choice = "End Phase";
        }
    }

    [PunRPC]
    void PathToForest(int cardID, int tileposition, bool flipped)
    {
        photonView.RPC("SendDiscard", RpcTarget.All, cardID);
        TileData x = Manager.instance.listoftiles[tileposition];

        Path newpath = PhotonView.Find(cardID).GetComponent<Path>();
        newpath.FlipCard(flipped);
        x.NewTile(newpath);
    }

    [PunRPC]
    void SendDiscard(int cardID)
    {
        Transform x = PhotonView.Find(cardID).transform;
        if (PhotonNetwork.IsMasterClient)
        {
            x.SetParent(Manager.instance.discard);
            x.localPosition = new Vector2(0, 0);
        }
        else
            x.SetParent(null);
    }

    public void DrawCardRPC(int cardsToDraw)
    {
        if (!hermit)
        {
            photonView.RPC("RequestDraw", RpcTarget.MasterClient, cardsToDraw, this.pv.Controller);
        }
    }

    [PunRPC]
    void RequestDraw(int cardsToDraw, Photon.Realtime.Player requestingplayer)
    {
        int[] cardIDs = new int[cardsToDraw];
        for (int i = 0; i<cardsToDraw; i++)
        {
            if (Manager.instance.deck.childCount == 0)
            {
                Manager.instance.discard.Shuffle();
                while (Manager.instance.discard.childCount > 0)
                    Manager.instance.discard.GetChild(0).SetParent(Manager.instance.deck);
            }

            PhotonView x = Manager.instance.deck.GetChild(i).GetComponent<PhotonView>();
            cardIDs[i] = x.ViewID;
            x.transform.SetParent(null);
        }

        photonView.RPC("SendDraw", requestingplayer, cardIDs);
    }

    [PunRPC]
    IEnumerator SendDraw(int[] cardIDs)
    {
        for (int i = 0; i < cardIDs.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            PhotonView.Find(cardIDs[i]).transform.SetParent(this.hand);
        }
        photonView.RPC("UpdateMyText", RpcTarget.All, hand.childCount);
    }

    [PunRPC]
    public void EnchantressAttack()
    {
        enchanted = true;
    }

    [PunRPC]
    public IEnumerator GuideChoice(Photon.Realtime.Player requestingplayer)
    {
        choicetext.transform.parent.gameObject.SetActive(true);
        choicetext.text = $"{this.name}: Choose a bonus for {requestingplayer.NickName}";
        choice = "";

        List<SendChoice> listofchoices = new List<SendChoice>();
        listofchoices.Add(CreateButton("+2 Draw"));
        listofchoices.Add(CreateButton("+2 Plays"));
        listofchoices.Add(CreateButton("+2 Moves"));

        while (choice == "")
            yield return null;
        for (int i = 0; i < listofchoices.Count; i++)
            Destroy(listofchoices[i].gameObject);
        choicetext.transform.parent.gameObject.SetActive(false);
        GetPreviousPlayer().photonView.RPC("ReceiveGuideChoice", requestingplayer, choice);
    }

    [PunRPC]
    void ReceiveGuideChoice(string choice)
    {
        Debug.Log(choice);
        switch (choice)
        {
            case "+2 Draw":
                DrawCardRPC(2);
                break;
            case "+2 Plays":
                AddPlays(2);
                break;
            case "+2 Moves":
                AddMoves(2);
                break;
            default:
                break;
        }
        photonView.RPC("WaitDone", photonrealtime);
    }

    [PunRPC]
    public IEnumerator ArcherAttack(Photon.Realtime.Player requestingplayer)
    {
        int cardstodiscard = hand.childCount / 2;
        for (int i = cardstodiscard; i>0; i--)
        {
            for (int j = 0; j < hand.childCount; j++)
            {
                SendChoice ct = hand.GetChild(j).GetComponent<SendChoice>();
                ct.EnableButton(this);
            }

            choicetext.transform.parent.gameObject.SetActive(true);
            choicetext.text = $"{this.name}: Discard a card ({i} more)";

            choice = "";
            chosencard = null;
            while (choice == "")
                yield return null;

            for (int j = 0; j < hand.childCount; j++)
                hand.GetChild(j).GetComponent<SendChoice>().DisableButton();
            choicetext.transform.parent.gameObject.SetActive(false);
            photonView.RPC("SendDiscard", RpcTarget.All, chosencard.pv.ViewID);
            photonView.RPC("UpdateMyText", RpcTarget.All, hand.childCount);
        }

        GameObject.Find(requestingplayer.NickName).GetComponent<Player>().photonView.RPC("WaitDone", requestingplayer);
    }

    public Player GetPreviousPlayer()
    {
        if (this.position == 0)
            return Manager.instance.playerordergameobject[Manager.instance.playerordergameobject.Count - 1];
        else
            return Manager.instance.playerordergameobject[this.position - 1];
    }

    public IEnumerator GameOver(string endtext)
    {
        while (choicehand.childCount > 0)
        {
            choicehand.GetChild(0).SetParent(null);
        }

        choicetext.transform.parent.gameObject.SetActive(true);
        choicetext.text = endtext + "\nReturning to the lobby...";
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("1. Lobby");
    }

}