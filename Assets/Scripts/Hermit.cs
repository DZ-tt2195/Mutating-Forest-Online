using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermit : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        yield return null;
        player.DrawCardRPC(3);
        player.AddPlays(1);
        player.hermit = true;
    }

}
