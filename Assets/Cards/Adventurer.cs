using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Adventurer : Explorer
{
    protected override void GetText()
    {
        this.name = "Adventurer";
        cardName.text = "Adventurer";
        cardText.text = "+2 Move\nPlay up to 2 Paths from your hand.";
        cardArtist.text = "Ryan Laukat (“Adventurer”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        player.AddMoves(2);

        for (int k = 0; k < 2; k++)
        {
            bool pathinhand = false;

            foreach (Card card in player.cardsInHand)
            {
                if (card.myType == CardType.Path)
                {
                    pathinhand = true;
                    card.choicescript.EnableButton(player);
                }
            }

            if (pathinhand)
            {
                player.choicetext.transform.parent.gameObject.SetActive(true);
                player.choicetext.text = $"{this.name}: Play a path?";
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
                    Path cardforlater = player.chosencard.GetComponent<Path>();

                    foreach (TileData tile in Manager.instance.listoftiles)
                    {
                        if (!tile.river)
                            tile.choicescript.EnableButton(player);
                    }

                    player.choice = "";
                    player.chosentile = null;
                    while (player.choice == "")
                        yield return null;

                    foreach (TileData tile in Manager.instance.listoftiles)
                        tile.choicescript.DisableButton();

                    player.photonView.RPC("PathToForest", RpcTarget.All, cardforlater.pv.ViewID, player.chosentile.position, cardforlater.flipped);
                    player.photonView.RPC("CreatePathGrid", RpcTarget.All, cardforlater.pv.ViewID);
                    player.cardsInHand.Remove(cardforlater);
                    player.SortHand();
                    yield break;
                }
            }
        }
    }
}
