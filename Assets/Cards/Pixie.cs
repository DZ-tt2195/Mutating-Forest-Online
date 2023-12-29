using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : Explorer
{
    protected override void GetText()
    {
        this.name = "Pixie";
        cardName.text = "Pixie";
        cardText.text = "+2 Play\nReveal your hand. If you didn’t reveal any Explorers, +2 Draw.";
        cardArtist.text = "Claus Stephan (“Pixie”)";
    }

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
        foreach (Card card in player.cardsInHand)
        {
            if (card.myType == CardType.Explorer)
                return false;
        }

        return true;
    }
}
