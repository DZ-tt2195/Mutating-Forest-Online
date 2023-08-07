using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Archer : Explorer
{
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
