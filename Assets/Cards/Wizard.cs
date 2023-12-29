using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Wizard : Explorer
{
    protected override void GetText()
    {
        this.name = "Wizard";
        cardName.text = "Wizard";
        cardText.text = "Each other player only gets 1 Play and 1 Move on their next turn (instead of the regular 2).";
        cardArtist.text = "Harald Lieske (“Sorcerer”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        yield return null;
        int playertracker = player.position;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextplayer = Manager.instance.playerordergameobject[playertracker];
            if (nextplayer.position != player.position)
                nextplayer.pv.RPC("EnchantressAttack", nextplayer.photonrealtime);
            playertracker = (playertracker == Manager.instance.playerordergameobject.Count - 1) ? 0 : playertracker + 1;
        }
    }
}
