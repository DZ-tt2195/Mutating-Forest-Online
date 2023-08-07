using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SendChoice : MonoBehaviour
{
    [HideInInspector] public Player choosingplayer;
    public Button button;
    [HideInInspector] public Card card;
    public TMP_Text textbox;
    Image border;
    [HideInInspector] public TileData chosentile;

    // Start is called before the first frame update
    void Start()
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
            border.color = new Color(1, 1, 1, Manager.instance.opacity);
        }
        else if (border != null && !button.interactable)
        {
            border.color = new Color(1, 1, 1, 0);
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
