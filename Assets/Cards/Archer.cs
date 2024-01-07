using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

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
                this.pv.RPC("DiscardHalf", nextplayer.photonrealtime, nextplayer.position, player.position);
                player.waiting = true;
                while (player.waiting)
                    yield return null;
            }
            playertracker = (playertracker == Manager.instance.playerordergameobject.Count - 1) ? 0 : playertracker + 1;
        }
    }

    [PunRPC]
    IEnumerator DiscardHalf(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerordergameobject[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerordergameobject[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, thisPlayer.name);

        int cardstodiscard = thisPlayer.cardsInHand.Count / 2;
        for (int i = cardstodiscard; i > 0; i--)
        {
            foreach (Card card in thisPlayer.cardsInHand)
                card.choicescript.EnableButton(thisPlayer);

            Manager.instance.instructions.text = $"{this.name}: Discard a card ({i} more)";

            thisPlayer.choice = "";
            thisPlayer.chosencard = null;
            while (thisPlayer.choice == "")
                yield return null;

            foreach (Card card in thisPlayer.cardsInHand)
                card.choicescript.DisableButton();

            thisPlayer.photonView.RPC("SendDiscard", RpcTarget.All, thisPlayer.chosencard.pv.ViewID);
            thisPlayer.photonView.RPC("UpdateMyText", RpcTarget.All, thisPlayer.cardsInHand.Count);
        }

        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerorderphotonlist[requestingPlayerPosition]);
    }
}
