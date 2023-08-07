using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Squire : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        bool explorerinhand = false;
        for (int i = 0; i < player.hand.childCount; i++)
        {
            SendChoice ct = player.hand.GetChild(i).GetComponent<SendChoice>();
            if (ct.card.explorer)
            {
                ct.EnableButton(player);
                explorerinhand = true;
            }
        }

        if (explorerinhand)
        {
            player.choicetext.transform.parent.gameObject.SetActive(true);
            player.choicetext.text = $"{this.name}: Play an explorer twice?";
            SendChoice x = player.CreateButton("No");

            player.choice = "";
            player.chosencard = null;
            while (player.choice == "")
                yield return null;

            Destroy(x.gameObject);
            for (int i = 0; i < player.hand.childCount; i++)
                player.hand.GetChild(i).GetComponent<SendChoice>().DisableButton();
            player.choicetext.transform.parent.gameObject.SetActive(false);

            if (player.chosencard != null)
            {
                Card playedcard = player.chosencard;
                player.photonView.RPC("CreateExplorerGrid", RpcTarget.All, playedcard.pv.ViewID);
                player.photonView.RPC("SendDiscard", RpcTarget.All, playedcard.pv.ViewID);

                yield return playedcard.GetComponent<Explorer>().PlayThis(player);
                yield return playedcard.GetComponent<Explorer>().PlayThis(player);
            }
        }
    }
}