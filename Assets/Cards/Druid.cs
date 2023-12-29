using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid : Explorer
{
    protected override void GetText()
    {
        this.name = "Druid";
        cardName.text = "Druid";
        cardText.text = "+1 Draw +1 Play +1 Move\nThis turn, when you move off of a Path, discard that Path.";
        cardArtist.text = "Grant Hansen (“Druid”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        yield return null;
        player.DrawCardRPC(1);
        player.AddPlays(1);
        player.AddMoves(1);
        player.druid = true;
    }

}
