using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Hireling : Explorer
{
    protected override void GetText()
    {
        this.name = "Hireling";
        cardName.text = "Hireling";
        cardText.text = "+1 Draw +1 Play\nYou may swap your current Path with a Path that you’re adjacent to (don’t move any players).";
        cardArtist.text = "Claus Stephan (“Hireling”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        player.DrawCardRPC(1);
        player.AddPlays(1);

        if (player.pawn.currenttile.mypath != null)
        {
            List<SendChoice> listoftilechoices = new List<SendChoice>();
            TileData nexttile = player.pawn.currenttile.left;
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
                player.choicetext.text = $"{this.name}: Swap paths?";
                player.choice = "";
                player.chosentile = null;

                List<SendChoice> listofchoices = new List<SendChoice>{player.CreateButton("No")};

                while (player.choice == "")
                    yield return null;
                for (int i = 0; i < listofchoices.Count; i++)
                    Destroy(listofchoices[i].gameObject);
                for (int i = 0; i < listoftilechoices.Count; i++)
                    listoftilechoices[i].DisableButton();
                player.choicetext.transform.parent.gameObject.SetActive(false);

                if (player.chosentile != null)
                    this.pv.RPC("SwapPaths", RpcTarget.All, player.pawn.currenttile.position, player.chosentile.position);
            }
        }
    }

    [PunRPC]
    void SwapPaths(int currentposition, int swappedposition)
    {
        TileData tile1 = Manager.instance.listoftiles[currentposition];
        TileData tile2 = Manager.instance.listoftiles[swappedposition];
        Path path1 = tile1.mypath;
        Path path2 = tile2.mypath;

        path1.NewHome(tile2);
        path2.NewHome(tile1);
    }

}
