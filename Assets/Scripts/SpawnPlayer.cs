using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SpawnPlayer : MonoBehaviour
{
    TMP_Text datatext;
    public GameObject playerprefab;
    PhotonView pv;

    bool cancalculate = false;
    int lastframe = 0;
    int lastupdate = 60;
    float[] framearray = new float[60];

    // Start is called before the first frame update
    void Start()
    {
        datatext = GameObject.Find("Server Data").GetComponent<TMP_Text>();
        GameObject x = PhotonNetwork.Instantiate(playerprefab.name, new Vector2(0, 0), Quaternion.identity);

        pv = x.GetComponent<PhotonView>();
        pv.Owner.NickName = PlayerPrefs.GetString("Username");
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom)
        datatext.text = "Room: " + PhotonNetwork.CurrentRoom.Name +
            "\nHost: " + PhotonNetwork.MasterClient.NickName +
            $"\nFrames/Second: {CalculateFrames()}" +
            $"\nServer Ping: {PhotonNetwork.GetPing()}";
    }

    int CalculateFrames()
    {
        framearray[lastframe] = Time.deltaTime;
        lastframe = (lastframe + 1);
        if (lastframe == 60)
        {
            lastframe = 0;
            float total = 0;
            for (int i = 0; i < framearray.Length; i++)
                total += framearray[i];
            lastupdate = (int)(framearray.Length / total);
            return lastupdate;
        }
        return lastupdate;
    }    
}
