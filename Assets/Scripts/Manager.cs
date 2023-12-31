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
using MyBox;

public class Manager : MonoBehaviour, IOnEventCallback
{
    public static Manager instance;
    public const byte AddNextPlayerEvent = 1;
    public const byte GameOverEvent = 2;

    [Foldout("Prefabs", true)]
        [SerializeField] TileData tileclone;
        [SerializeField] PlayerButton playerbuttonclone;
        public Sprite cardback;

    [Foldout("UI", true)]
        [SerializeField] internal Transform cardsplayedgrid;
        Transform storePlayers;
        Transform gameboard;

    [Foldout("Lists", true)]
        [SerializeField] List<Pawn> listofpawns = new List<Pawn>();
        [ReadOnly] public List<TileData> listoftiles = new List<TileData>();
        [ReadOnly] public List<Player> playerordergameobject = new List<Player>();
        [ReadOnly] public List<Photon.Realtime.Player> playerorderphotonlist = new List<Photon.Realtime.Player>();

    [Foldout("Text", true)]
        [ReadOnly] public TMP_Text instructions;
        [ReadOnly] public TMP_Text numbers;
        [ReadOnly] public Transform deck;
        [ReadOnly] public Transform discard;

    [Foldout("Animation", true)]
        [ReadOnly] public float opacity = 1;
        [ReadOnly] public bool decrease = true;
        [ReadOnly] public bool gameon = false;

    void Awake()
    {
        instance = this;
        instructions = GameObject.Find("Instructions").GetComponent<TMP_Text>();
        numbers = GameObject.Find("Numbers").GetComponent<TMP_Text>();
        deck = GameObject.Find("Deck").transform;
        discard = GameObject.Find("Discard").transform;
        gameboard = GameObject.Find("Forest").transform;
        storePlayers = GameObject.Find("Store Players").transform;
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

    public TileData GetPosition(int r, int c)
    {
        if (r < 0 || r > 8 || c < 0 || c > 4)
            return null;
        else
            return listoftiles[(r) * 5 + (c)];
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && gameon && storePlayers.transform.childCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            GameOver("A player has disconnected.");
        }
    }

    IEnumerator WaitForPlayer()
    {
        while (storePlayers.transform.childCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            instructions.text = $"Waiting for more players ({PhotonNetwork.PlayerList.Length}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
            yield return null;
        }

        instructions.text = "Connected! Setting up...";
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient)
        {
            deck.Shuffle();

            List<int> startingpositions = new() { 0,4,40,44};

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

            PlayerButton playerbutton = Instantiate(playerbuttonclone, GameObject.Find("Canvas").transform);
            playerbutton.transform.localPosition = new Vector3(-1100, 425-80*playerposition, 0);
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
        object[] message = new object[1];
        message[0] = $"{PhotonNetwork.LocalPlayer.NickName} has resigned.";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameOverEvent, message, raiseEventOptions, SendOptions.SendReliable);
    }

    public void GameOver(string endMessage)
    {
        object[] message = new object[1];
        message[0] = endMessage;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameOverEvent, message, raiseEventOptions, SendOptions.SendReliable);
    }
}
