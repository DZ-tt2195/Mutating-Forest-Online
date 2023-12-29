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
        cardName = transform.Find("Name").GetComponent<TMP_Text>();
        cardText = transform.Find("Description").GetComponent<TMP_Text>();
        cardArtist = transform.Find("Artist").GetComponent<TMP_Text>();
        GetText();
    }

    protected virtual void GetText()
    {

    }

    public virtual IEnumerator PlayThis(Player player)
    {
        yield return null;
    }
}