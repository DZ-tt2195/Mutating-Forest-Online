using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using MyBox;

public class Card : MonoBehaviour
{
    [ReadOnly] public PhotonView pv;
    public enum CardType { Path, Explorer};
    [ReadOnly] public CardType myType;
    [ReadOnly] public Image image;
    [ReadOnly] public SendChoice choicescript;

    void Awake()
    {
        pv = this.GetComponent<PhotonView>();
        image = GetComponent<Image>();
        choicescript = GetComponent<SendChoice>();
    }

}
