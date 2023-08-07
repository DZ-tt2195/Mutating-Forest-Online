using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [HideInInspector] public PhotonView pv;
    [HideInInspector] public bool explorer;
    [HideInInspector] public bool path;
    [HideInInspector] public Image image;
    [HideInInspector] public SendChoice choicescript;

    // Start is called before the first frame update
    void Start()
    {
        pv = this.GetComponent<PhotonView>();
        image = GetComponent<Image>();
        choicescript = GetComponent<SendChoice>();
    }

}
