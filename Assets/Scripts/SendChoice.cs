using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;

public class SendChoice : MonoBehaviour
{
    [ReadOnly] public Player choosingplayer;
    [ReadOnly] public Button button;
    [ReadOnly] public Card card;
    [SerializeField] public TMP_Text textbox;
    Image border;
    [ReadOnly] public TileData chosentile;

    void Awake()
    {
        card = this.GetComponent<Card>();
        chosentile = this.GetComponent<TileData>();
        button.onClick.AddListener(SendName);
        if (card != null || chosentile != null)
            border = this.transform.GetChild(0).GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        if (border != null && button.interactable)
        {
            border.SetAlpha(Manager.instance.opacity);
        }
        else if (border != null && !button.interactable)
        {
            border.SetAlpha(0);
        }
    }

    public void EnableButton(Player player)
    {
        this.gameObject.SetActive(true);
        button.interactable = true;
        choosingplayer = player;
    }

    public void DisableButton()
    {
        button.interactable = false;
    }

    public void SendName()
    {
        if (textbox != null)
            choosingplayer.choice = textbox.text;
        else
            choosingplayer.choice = this.name;

        choosingplayer.chosencard = card;
        choosingplayer.chosentile = chosentile;
    }

}
