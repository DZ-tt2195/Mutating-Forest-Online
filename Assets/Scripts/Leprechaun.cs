using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Leprechaun : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        player.AddPlays(-1);
        while (player.hand.childCount > 0)
        {
            yield return new WaitForSeconds(0.1f);
            PhotonView nextcard = player.hand.GetChild(0).GetComponent<PhotonView>();
            player.pv.RPC("SendDiscard", Photon.Pun.RpcTarget.All, nextcard.ViewID);
        }
        player.DrawCardRPC(5);
    }
}
