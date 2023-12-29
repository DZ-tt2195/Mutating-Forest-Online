using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermit : Explorer
{
    protected override void GetText()
    {
        this.name = "Hermit";
        cardName.text = "Hermit";
        cardText.text = "+3 Draw +1 Play\nIgnore any further +Draw you get this turn (including the free draw at the end of your turn).";
        cardArtist.text = "Claus Stephan (“Hermit”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        yield return null;
        player.DrawCardRPC(3);
        player.AddPlays(1);
        player.hermit = true;
    }

}
