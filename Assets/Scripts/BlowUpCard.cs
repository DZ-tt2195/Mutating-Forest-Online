using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlowUpCard : MonoBehaviour
{
    public static BlowUpCard instance;
    [SerializeField] CardDisplay bigCard;

    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            bigCard.transform.parent.gameObject.SetActive(false);
    }

    public void ChangeCard(Explorer explorer)
    {
        bigCard.transform.parent.gameObject.SetActive(true);
        bigCard.CardArt(explorer);
    }

    public void ChangeCard(Path path)
    {
        bigCard.transform.parent.gameObject.SetActive(true);
        bigCard.CardArt(path);
    }
}
