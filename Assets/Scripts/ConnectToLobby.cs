using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToLobby : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
        PhotonNetwork.ConnectUsingSettings();
        PlayerPrefs.SetString("Username", "");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("1. Lobby");
    }
}
