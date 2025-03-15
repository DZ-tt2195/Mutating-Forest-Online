using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using MyBox;
using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

#region Variables

    [Foldout("Online", true)]
    [SerializeField] Transform onlineFields;
    [SerializeField] TMP_InputField join;
    [SerializeField] TMP_InputField username;

    [Foldout("Local", true)]
    [SerializeField] Transform localFields;
    [SerializeField] TMP_InputField player1;
    [SerializeField] TMP_InputField player2;

    [Foldout("General", true)]
    [SerializeField] TMP_Text error;

    #endregion

#region Setup

    private void Start()
    {
        if (PlayerPrefs.HasKey("P1"))
            player1.text = PlayerPrefs.GetString("P1");
        if (PlayerPrefs.HasKey("P2"))
            player2.text = PlayerPrefs.GetString("P2");
        if (PlayerPrefs.HasKey("Online Username"))
            username.text = PlayerPrefs.GetString("Online Username");

        error.gameObject.SetActive(false);
        onlineFields.gameObject.SetActive(PhotonNetwork.IsConnected);
        localFields.gameObject.SetActive(!PhotonNetwork.IsConnected);
    }

    IEnumerator ErrorMessage(string x)
    {
        error.gameObject.SetActive(true);
        error.text = x;
        yield return new WaitForSeconds(3f);
        error.gameObject.SetActive(false);
    }

    #endregion

#region Loading Scene

    public void PlayGame()
    {
        PlayerPrefs.SetString("P1", player1.text);
        PlayerPrefs.SetString("P2", player2.text);
        SceneManager.LoadScene("2. Game");
    }

    public void CreateRoom(int playerCount)
    {
        if (username.text == "")
        {
            StartCoroutine(ErrorMessage("You forgot to type in a username."));
        }
        else
        { 
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)playerCount;
            PhotonNetwork.CreateRoom(username.text.ToUpper(), roomOptions, null);
        }
    }

    public void JoinRoom()
    {
        if (join.text == "")
        {
            StartCoroutine(ErrorMessage("You forgot to type in the host's name."));
        }
        else if (username.text == "")
        {
            StartCoroutine(ErrorMessage("You forgot to type in a username."));
        }
        PhotonNetwork.JoinRoom(join.text.ToUpper());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(ErrorMessage("That room already exists."));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        StartCoroutine(ErrorMessage("That room is already full."));
    }

    public override void OnJoinedRoom()
    {
        PlayerPrefs.SetString("Online Username", username.text);
        PhotonNetwork.LoadLevel("2. Game");
    }

#endregion

}
