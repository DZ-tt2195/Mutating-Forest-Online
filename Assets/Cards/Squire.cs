using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Squire : Explorer
{
    protected override void GetText()
    {
        this.name = "Squire";
        cardName.text = "Squire";
        cardText.text = "You may play an Explorer from your hand twice.";
        cardArtist.text = "Anthony Palumbo (“Seekers’ Squire”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        bool explorerinhand = false;
        foreach (Card card in player.cardsInHand)
        {
            if (card.myType == CardType.Explorer)
            {
                card.choicescript.EnableButton(player);
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

            foreach (Card card in player.cardsInHand)
                card.choicescript.DisableButton();
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