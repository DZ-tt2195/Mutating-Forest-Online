using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Enchantress : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        yield return null;

        int playertracker = player.position;

        for (int i = 0; i<PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextplayer = Manager.instance.playerordergameobject[playertracker];
            if (nextplayer.position != player.position)
            {
                nextplayer.pv.RPC("EnchantressAttack", nextplayer.photonrealtime);
            }
            playertracker = (playertracker == Manager.instance.playerordergameobject.Count - 1) ? 0 : playertracker + 1;
        }
    }
}
