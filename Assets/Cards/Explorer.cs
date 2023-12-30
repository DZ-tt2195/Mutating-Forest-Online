using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using MyBox;

public class Explorer : Card, IPointerClickHandler
{
    [ReadOnly] public TMP_Text cardName { get; private set; }
    [ReadOnly] public TMP_Text cardText { get; private set; }
    [ReadOnly] public TMP_Text cardArtist { get; private set; }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BlowUpCard.instance.ChangeCard(this);
        }
    }
}