using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Explorer : Card
{
    protected TMP_Text cardName;
    protected TMP_Text cardText;
    protected TMP_Text cardArtist;

    private void Start()
    {
        myType = CardType.Explorer;
        cardName = this.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>();
        cardText = this.transform.GetChild(1).GetChild(3).GetComponent<TMP_Text>();
        cardArtist = this.transform.GetChild(1).GetChild(4).GetComponent<TMP_Text>();
        GetText();
    }

    protected virtual void GetText()
    {

    }

    public virtual IEnumerator PlayThis(Player player)
    {
        yield return null;
    }

    protected override void SetSprite()
    {
        canvasgroup.alpha = 1;
        image.sprite = null;
        image.color = Color.black;
    }
}