using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using MyBox;
using static Card;
using System.Linq;
using System;

public class Player : MonoBehaviourPunCallbacks
{

#region Variables

    [Foldout("Prefabs", true)]
        public SendChoice buttonprefab;
        public CardDisplay carddisplay;

    [Foldout("Cards", true)]
        Transform hand;
        Canvas canvas;
        [ReadOnly] public List<Card> cardsInHand = new List<Card>();

    [Foldout("Player Info", true)]
        [ReadOnly] public Pawn pawn;
        public PhotonView pv;
        PlayerButton playerbutton;
        [ReadOnly] public int position;
        [ReadOnly] public Photon.Realtime.Player photonrealtime;

    [Foldout("Decisions", true)]
        [ReadOnly] public TMP_Text choicetext;
        [ReadOnly] public Transform choicehand;
        [ReadOnly] public string choice;
        [ReadOnly] public Card chosencard;
        [ReadOnly] public TileData chosentile;
        enum TypeOfCard { None, Any, Paths, Explorers };

    [Foldout("Turns", true)]
        SendChoice endturnbutton;
        [ReadOnly] public bool druid = false;
        [ReadOnly] public bool enchanted = false;
        [ReadOnly] public bool waiting = false;
        [ReadOnly] bool turnon = false;
        [ReadOnly] public bool hermit = false;
        bool didnothing;
        [ReadOnly] public int plays;
        [ReadOnly] public int moves;

    #endregion

#region Setup

    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        this.transform.SetParent(GameObject.Find("Store Players").transform);
        this.transform.localPosition = new Vector2(0, 0);
        hand = this.transform.GetChild(0).transform;

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
    public void StartPawn(string buttonname, int pawnID, int startingposition)
    {
        pawn = PhotonView.Find(pawnID).GetComponent<Pawn>();
        pawn.SetStart(this, startingposition);
        playerbutton = GameObject.Find(buttonname).GetComponent<PlayerButton>();
        playerbutton.MoveBar();
    }

    #endregion

#region Decisions

    [PunRPC]
    public IEnumerator ClearGrid()
    {
        List<GameObject> list = new();
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
    public void CreatePathGrid(int cardID)
    {
        Path newpath = PhotonView.Find(cardID).GetComponent<Path>();
        CardDisplay cd = Instantiate(carddisplay, Manager.instance.cardsplayedgrid.transform);
        cd.CardArt(newpath);
    }

    public SendChoice CreateButton(string x)
    {
        SendChoice y = Instantiate(buttonprefab, choicehand);
        y.textbox.text = x;
        y.name = x;
        y.EnableButton(this);
        return y;
    }

    public void FlipButton()
    {
        for (int i = 0; i<hand.childCount; i++)
        {
            if (hand.GetChild(i).TryGetComponent<Path>(out var nextcard))
                nextcard.FlipCard();
        }
    }

    #endregion

#region Numbers

    public void AddPlays(int n)
    {
        plays += n;
        pv.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
        pv.RPC("UpdateMyText", RpcTarget.All, cardsInHand.Count);
    }

    public void AddMoves(int n)
    {
        moves += n;
        pv.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
        pv.RPC("UpdateMyText", RpcTarget.All, cardsInHand.Count);
    }

    [PunRPC]
    public void UpdateNumbers(int plays, int moves)
    {
        Manager.instance.numbers.text = $"Plays: {plays}, Moves: {moves}";
    }

    [PunRPC]
    public void UpdateMyText(int cardsinhand)
    {
        playerbutton.textbox.text = $"{this.name}: {cardsinhand} Cards";
    }

    #endregion

#region Turns

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
    IEnumerator TakeTurn()
    {
        if (pv.IsMine)
        {
            photonView.RPC("WaitForPlayer", RpcTarget.Others, this.name);
            didnothing = true;
            endturnbutton.EnableButton(this);
            bool continueturn = true;

            plays = (enchanted) ? 1 : 2;
            moves = (enchanted) ? 1 : 2;

            while (plays > 0 && continueturn)
            {
                yield return new WaitForSeconds(0.2f);
                photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
                photonView.RPC("WaitForPlayer", RpcTarget.Others, this.name);
                Manager.instance.instructions.text = $"Your Turn - Play Explorers";

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
                    AddPlays(-1);
                    didnothing = false;
                    endturnbutton.DisableButton();
                }
            }

            while (plays > 0 && continueturn)
            {
                yield return new WaitForSeconds(0.2f);
                photonView.RPC("UpdateNumbers", RpcTarget.All, plays, moves);
                Manager.instance.instructions.text = $"Your Turn - Play Paths";
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
                Manager.instance.instructions.text = $"Your Turn - Move";
                photonView.RPC("WaitForPlayer", RpcTarget.Others, this.name);

                yield return AskMove();
                if (choice == "Do Nothing")
                {
                    didnothing = true;
                    endturnbutton.DisableButton();
                    break;
                }
                else if (choice == "End Phase")
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

            DrawCardRPC(didnothing ? 2 : 1);

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

    IEnumerator AskExplorer()
    {
        if (pv.IsMine)
        {
            bool explorerinhand = false;
            foreach (Card card in this.cardsInHand)
            {
                if (card.myType == CardType.Explorer)
                {
                    card.choicescript.EnableButton(this);
                    explorerinhand = true;
                }
            }

            if (explorerinhand)
            {
                this.choicetext.transform.parent.gameObject.SetActive(true);
                this.choicetext.text = $"{this.name}: Play an explorer?";
                SendChoice x = this.CreateButton("No");

                this.choice = "";
                this.chosencard = null;
                while (this.choice == "")
                    yield return null;

                Destroy(x.gameObject);

                foreach (Card card in this.cardsInHand)
                    card.choicescript.DisableButton();
                this.choicetext.transform.parent.gameObject.SetActive(false);

                if (this.chosencard != null)
                {
                    Card playedcard = this.chosencard;
                    this.photonView.RPC("CreateExplorerGrid", RpcTarget.All, playedcard.pv.ViewID);
                    this.photonView.RPC("SendDiscard", RpcTarget.All, playedcard.pv.ViewID);

                    yield return playedcard.GetComponent<Explorer>().PlayThis(this);
                    yield break;
                }
            }

            if (choice != "Do Nothing")
                choice = "End Phase";
        }
    }

    IEnumerator AskPath()
    {
        if (pv.IsMine)
        {
            bool pathinhand = false;

            foreach (Card card in this.cardsInHand)
            {
                if (card.myType == CardType.Path)
                {
                    pathinhand = true;
                    card.choicescript.EnableButton(this);
                }
            }

            if (pathinhand)
            {
                this.choicetext.transform.parent.gameObject.SetActive(true);
                this.choicetext.text = $"{this.name}: Play a path?";
                SendChoice x = this.CreateButton("No");

                this.choice = "";
                this.chosencard = null;
                while (this.choice == "")
                    yield return null;

                Destroy(x.gameObject);
                foreach (Card card in this.cardsInHand)
                    card.choicescript.DisableButton();
                this.choicetext.transform.parent.gameObject.SetActive(false);

                if (this.chosencard != null)
                {
                    Path cardforlater = this.chosencard.GetComponent<Path>();

                    foreach (TileData tile in Manager.instance.listoftiles)
                    {
                        if (!tile.river)
                            tile.choicescript.EnableButton(this);
                    }

                    this.choice = "";
                    this.chosentile = null;
                    while (this.choice == "")
                        yield return null;

                    foreach (TileData tile in Manager.instance.listoftiles)
                        tile.choicescript.DisableButton();

                    this.photonView.RPC("PathToForest", RpcTarget.All, cardforlater.pv.ViewID, this.chosentile.position, cardforlater.flipped);
                    this.photonView.RPC("CreatePathGrid", RpcTarget.All, cardforlater.pv.ViewID);
                    cardsInHand.Remove(cardforlater);
                    SortHand();
                    yield break;
                }
            }

            if (choice != "Do Nothing")
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

                List<SendChoice> listofchoices = new List<SendChoice> { CreateButton("Yes"), CreateButton("No") };

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
                    yield break;
                }
            }

            if (choice != "Do Nothing")
                choice = "End Phase";
        }
    }

    [PunRPC]
    void PathToForest(int cardID, int tileposition, bool flipped)
    {
        TileData newTile = Manager.instance.listoftiles[tileposition];
        Path newPath;

        if (newTile.mypath != null)
            this.photonView.RPC("SendDiscard", RpcTarget.All, newTile.mypath.pv.ViewID);

        try
        {
            newPath = PhotonView.Find(cardID).GetComponent<Path>();
            cardsInHand.Remove(newPath);
            newPath.NewHome(newTile);
        }
        catch (NullReferenceException)
        {
            newTile.mypath = null;
        }
    }

#endregion

#region Cards

    [PunRPC]
    void SendDiscard(int cardID)
    {
        Card discardMe = PhotonView.Find(cardID).GetComponent<Card>();
        cardsInHand.Remove(discardMe);
        SortHand();

        discardMe.transform.SetParent(Manager.instance.discard);
        StartCoroutine(discardMe.MoveCard(new Vector2(-2000, -330), new Vector3(0, 0, 0), 0.3f));
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
        }

        photonView.RPC("SendDraw", RpcTarget.All, cardIDs);
    }

    [PunRPC]
    void SendDraw(int[] cardIDs)
    {
        for (int i = 0; i < cardIDs.Length; i++)
        {
            Card newCard = PhotonView.Find(cardIDs[i]).GetComponent<Card>();
            newCard.transform.SetParent(this.hand);
            newCard.transform.localPosition = new Vector2(0, -1100);
            cardsInHand.Add(newCard);
            newCard.image.sprite = Manager.instance.cardback;
        }
        SortHand();
    }

    public void SortHand()
    {
        float firstCalc = Mathf.Round(canvas.transform.localScale.x * 4) / 4f;
        float multiplier = firstCalc / 0.25f;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Card nextCard = cardsInHand[i];
            float startingX = (cardsInHand.Count > 7) ? (-300 - (150 * multiplier)) : (cardsInHand.Count-1) * (-50 - 25 * multiplier);
            float difference = (cardsInHand.Count > 7) ? (-300 - (150 * multiplier)) * -2 / (cardsInHand.Count-1) : 100 + (50 * multiplier);
            Vector2 newPosition = new(startingX + difference * i, -515 * canvas.transform.localScale.x);
            StartCoroutine(nextCard.MoveCard(newPosition, nextCard.transform.localEulerAngles, 0.3f));
        }

        foreach (Card card in cardsInHand)
            StartCoroutine(card.RevealCard(0.3f));

        photonView.RPC("UpdateMyText", RpcTarget.All, cardsInHand.Count);
    }

    #endregion

#region Misc

    [PunRPC]
    public void EnchantressAttack()
    {
        enchanted = true;
    }

    public Player GetPreviousPlayer()
    {
        if (this.position == 0)
            return Manager.instance.playerordergameobject[^1];
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

    #endregion

}