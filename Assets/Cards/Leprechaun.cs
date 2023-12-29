using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Leprechaun : Explorer
{
    protected override void GetText()
    {
        this.name = "Leprechaun";
        cardName.text = "Leprechaun";
        cardText.text = "-1 Play\nDiscard your hand. +5 Draw.";
        cardArtist.text = "Brian Brinlee (“Leprechaun”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        player.AddPlays(-1);
        while (player.cardsInHand.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            player.pv.RPC("SendDiscard", RpcTarget.All, player.cardsInHand[0].pv.ViewID);
        }
        player.DrawCardRPC(5);
    }
}
