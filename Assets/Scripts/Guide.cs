using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        player.AddPlays(1);
        Player prevPlayer = player.GetPreviousPlayer();

        if (prevPlayer != null)
            prevPlayer = player;

        prevPlayer.photonView.RPC("GuideChoice", prevPlayer.photonrealtime, player.photonrealtime);
        player.waiting = true;
        while (player.waiting)
        {
            yield return null;
        }
    }
}
