using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        yield return null;
        player.AddPlays(2);

        if (NoExplorers(player))
        {
            player.DrawCardRPC(2);
        }

    }

    bool NoExplorers(Player player)
    {
        for (int i = 0; i < player.hand.childCount; i++)
        {
            if (player.hand.transform.GetChild(i).GetComponent<Card>().explorer)
                return false;
        }
        return true;
    }
}
