using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        yield return null;
        player.DrawCardRPC(1);
        player.AddPlays(1);
        player.AddMoves(1);
        player.druid = true;
    }

}
