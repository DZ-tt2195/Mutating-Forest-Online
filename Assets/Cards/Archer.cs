using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Archer : Explorer
{
    protected override void GetText()
    {
        this.name = "Archer";
        cardName.text = "Archer";
        cardText.text = "Each other player discards half of their hand (round down).";
        cardArtist.text = "Harald Lieske (“Archer”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        int playertracker = player.position;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextplayer = Manager.instance.playerordergameobject[playertracker];
            if (nextplayer.position != player.position)
            {
                nextplayer.pv.RPC("ArcherAttack", nextplayer.photonrealtime, player.photonrealtime);
                player.waiting = true;
                while (player.waiting)
                    yield return null;
            }
            playertracker = (playertracker == Manager.instance.playerordergameobject.Count - 1) ? 0 : playertracker + 1;
        }
    }
}
