using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Linq;
using System;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class Manager : MonoBehaviour, IOnEventCallback
{
    public static Manager instance;
    public TileData tileclone;
    public PlayerButton playerbuttonclone;
    public Image blownUp;
    public Transform cardsplayedgrid;
    public List<Pawn> listofpawns = new List<Pawn>();
    [HideInInspector] public List<TileData> listoftiles = new List<TileData>();

    [HideInInspector] public TMP_Text instructions;
    [HideInInspector] public TMP_Text numbers;
    [HideInInspector] public Transform cardback;
    [HideInInspector] public Transform deck;
    [HideInInspector] public Transform discard;
    Transform gameboard;

    [HideInInspector] public List<Player> playerordergameobject = new List<Player>();
    [HideInInspector] public List<Photon.Realtime.Player> playerorderphotonlist = new List<Photon.Realtime.Player>();

    [HideInInspector] public float opacity = 1;
    [HideInInspector] public bool decrease = true;
    public bool gameon = false;

    public const byte AddNextPlayerEvent = 1;
    public const byte GameOverEvent = 2;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        instructions = GameObject.Find("Instructions").GetComponent<TMP_Text>();
        numbers = GameObject.Find("Numbers").GetComponent<TMP_Text>();
        cardback = GameObject.Find("Cardback").transform;
        deck = GameObject.Find("Deck").transform;
        discard = GameObject.Find("Discard").transform;
        gameboard = GameObject.Find("Forest").transform;
        blownUp.transform.parent.gameObject.SetActive(false);
    }

    void Start()
    { 
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                TileData nexttile = Instantiate(tileclone, gameboard);
                nexttile.row = i;
                nexttile.column = j;
                nexttile.name = $"{i}:{j}";
                nexttile.position = listoftiles.Count;
                listoftiles.Add(nexttile);
            }
        }

        for (int i = 0; i < listoftiles.Count; i++)
        {
            TileData x = listoftiles[i];
            x.up = GetPosition(x.row - 1, x.column);
            x.left = GetPosition(x.row, x.column - 1);
            x.down = GetPosition(x.row + 1, x.column);
            x.right = GetPosition(x.row, x.column + 1);
        }

        GetPosition(3, 0).SetRiver();
        GetPosition(3, 1).SetRiver();
        GetPosition(3, 3).SetRiver();
        GetPosition(3, 4).SetRiver();
        GetPosition(4, 0).SetRiver();
        GetPosition(4, 1).SetRiver();
        GetPosition(4, 3).SetRiver();
        GetPosition(4, 4).SetRiver();
        GetPosition(5, 0).SetRiver();
        GetPosition(5, 1).SetRiver();
        GetPosition(5, 3).SetRiver();
        GetPosition(5, 4).SetRiver();

        if (PhotonNetwork.IsMasterClient)
            deck.Shuffle();
        StartCoroutine(WaitForPlayer());
    }

    private void FixedUpdate()
    {
        if (decrease)
            opacity -= 0.05f;
        else
            opacity += 0.05f;
        if (opacity < 0 || opacity > 1)
            decrease = !decrease;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            blownUp.transform.parent.gameObject.SetActive(false);
    }

    public TileData GetPosition(int r, int c)
    {
        if (r < 0 || r > 8 || c < 0 || c > 4)
            return null;
        else
            return listoftiles[(r) * 5 + (c)];
    }

    IEnumerator WaitForPlayer()
    {
        GameObject x = GameObject.Find("Store Players").gameObject;
        while (x.transform.childCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            instructions.text = $"Waiting for more players ({PhotonNetwork.PlayerList.Length}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
            yield return null;
        }

        instructions.text = "Connected! Setting up...";
        yield return new WaitForSeconds(2f);

        if (PhotonNetwork.IsMasterClient)
        {
            deck.Shuffle();

            List<int> startingpositions = new List<int>();
            startingpositions.Add(0);
            startingpositions.Add(4);
            startingpositions.Add(40);
            startingpositions.Add(44);

            List<Photon.Realtime.Player> playerassignment = new List<Photon.Realtime.Player>();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                playerassignment.Add(PhotonNetwork.PlayerList[i]);

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                object[] sendingdata = new object[3];
                sendingdata[0] = i;

                int randomremove = UnityEngine.Random.Range(0, startingpositions.Count);
                sendingdata[1] = startingpositions[randomremove];
                startingpositions.RemoveAt(randomremove);

                randomremove = UnityEngine.Random.Range(0, playerassignment.Count);
                sendingdata[2] = playerassignment[randomremove];
                playerassignment.RemoveAt(randomremove);

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(AddNextPlayerEvent, sendingdata, raiseEventOptions, SendOptions.SendReliable);
            }

            yield return new WaitForSeconds(1f);
            for (int i = 0; i < playerordergameobject.Count; i++)
                playerordergameobject[i].DrawCardRPC(4);

            gameon = true;
            while (gameon)
            {
                for (int i = 0; i < playerordergameobject.Count; i++)
                {
                    yield return new WaitForSeconds(0.5f);
                    yield return playerordergameobject[i].TakeTurnRPC(playerordergameobject[i].photonrealtime);
                }

            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == AddNextPlayerEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int playerposition = (int)data[0];
            int startingposition = (int)data[1];
            Photon.Realtime.Player playername = (Photon.Realtime.Player)data[2];

            playerordergameobject.Add(GameObject.Find(playername.NickName).GetComponent<Player>());
            playerorderphotonlist.Add(playername);

            playerordergameobject[playerposition].position = playerposition;
            playerordergameobject[playerposition].photonrealtime = playername;

            PlayerButton playerbutton = Instantiate(playerbuttonclone, GameObject.Find("CanvasObject").transform);
            playerbutton.transform.localPosition = new Vector3(-810, 290-30*playerposition, 0);
            playerbutton.name = $"{playername.NickName}'s Button";
            playerbutton.image.color = listofpawns[playerposition].image.color;
            playerbutton.Setup(listofpawns[playerposition]);
            playerordergameobject[playerposition].photonView.RPC("StartPawn", RpcTarget.All, playerbutton.name, listofpawns[playerposition].pv.ViewID, startingposition);
        }

        if (photonEvent.Code == GameOverEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            gameon = false;
            StopAllCoroutines();
            StartCoroutine(GameObject.Find($"{PhotonNetwork.LocalPlayer.NickName}").GetComponent<Player>().GameOver((string)data[0]));
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void Resign()
    {
        object[] lol = new object[1];
        lol[0] = $"{PhotonNetwork.LocalPlayer.NickName} has resigned.";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameOverEvent, lol, raiseEventOptions, SendOptions.SendReliable);
    }

    public void YouWon(string playername)
    {
        object[] lol = new object[1];
        lol[0] = $"{playername} has won!";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameOverEvent, lol, raiseEventOptions, SendOptions.SendReliable);
    }
}
