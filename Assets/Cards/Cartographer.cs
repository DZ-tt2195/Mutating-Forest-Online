using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cartographer : Explorer
{
    protected override void GetText()
    {
        this.name = "Cartographer";
        cardName.text = "Cartographer";
        cardText.text = "+2 Draw\nEither rotate your current Path, or rotate a Path you’re adjacent to.";
        cardArtist.text = "Mark Poole (“Cartographer”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        player.DrawCardRPC(2);

        List<SendChoice> listoftilechoices = new List<SendChoice>();
        TileData nexttile = player.pawn.currenttile;
        if (nexttile != null && nexttile.mypath != null)
        {
            nexttile.choicescript.EnableButton(player);
            listoftilechoices.Add(nexttile.choicescript);
        }
        nexttile = player.pawn.currenttile.left;
        if (nexttile != null && nexttile.mypath != null)
        {
            nexttile.choicescript.EnableButton(player);
            listoftilechoices.Add(nexttile.choicescript);
        }
        nexttile = player.pawn.currenttile.right;
        if (nexttile != null && nexttile.mypath != null)
        {
            nexttile.choicescript.EnableButton(player);
            listoftilechoices.Add(nexttile.choicescript);
        }
        nexttile = player.pawn.currenttile.up;
        if (nexttile != null && nexttile.mypath != null)
        {
            nexttile.choicescript.EnableButton(player);
            listoftilechoices.Add(nexttile.choicescript);
        }
        nexttile = player.pawn.currenttile.down;
        if (nexttile != null && nexttile.mypath != null)
        {
            nexttile.choicescript.EnableButton(player);
            listoftilechoices.Add(nexttile.choicescript);
        }

        if (listoftilechoices.Count > 0)
        {
            player.choicetext.transform.parent.gameObject.SetActive(true);
            player.choicetext.text = $"{this.name}: Rotate a path?";
            player.choice = "";
            player.chosentile = null;

            List<SendChoice> listofchoices = new List<SendChoice>();
            listofchoices.Add(player.CreateButton("No"));

            while (player.choice == "")
                yield return null;

            for (int i = 0; i < listofchoices.Count; i++)
                Destroy(listofchoices[i].gameObject);
            for (int i = 0; i < listoftilechoices.Count; i++)
                listoftilechoices[i].DisableButton();

            player.choicetext.transform.parent.gameObject.SetActive(false);
            if (player.chosentile != null)
            {
                this.pv.RPC("FlipPath", RpcTarget.All, player.chosentile.position);
            }
        }
    }
}
