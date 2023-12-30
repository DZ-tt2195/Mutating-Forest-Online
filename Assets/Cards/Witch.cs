using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Witch : Explorer
{
    protected override void GetText()
    {
        this.name = "Witch";
        cardName.text = "Witch";
        cardText.text = "+1 Play +1 Move\nTake a Path with no players on it and move it to another blank spot.";
        cardArtist.text = "Hans Krill (“Sorceress”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        yield return null;

        player.AddPlays(1);
        player.AddMoves(1);

        List<TileData> noplayers = new List<TileData>();
        List<TileData> blanktile = new List<TileData>();

        for (int i = 0; i<Manager.instance.listoftiles.Count; i++)
        {
            TileData nexttile = Manager.instance.listoftiles[i];
            if (!nexttile.river && nexttile.mypath != null && nexttile.PlayersOnThis().Count == 0)
                noplayers.Add(nexttile);
            else if (nexttile.mypath == null)
                blanktile.Add(nexttile);
        }

        if (noplayers.Count > 0 && blanktile.Count > 0)
        {
            player.choicetext.transform.parent.gameObject.SetActive(true);
            player.choicetext.text = $"{this.name}: Choose a path with no players";
            player.choice = "";
            player.chosentile = null;

            for (int i = 0; i < noplayers.Count; i++)
                noplayers[i].choicescript.EnableButton(player);
            while (player.choice == "")
                yield return null;
            for (int i = 0; i < noplayers.Count; i++)
                noplayers[i].choicescript.DisableButton();

            TileData storedtile = player.chosentile;

            player.choicetext.text = $"{this.name}: Move it to a blank spot";
            player.choice = "";
            player.chosentile = null;

            for (int i = 0; i < blanktile.Count; i++)
                blanktile[i].choicescript.EnableButton(player);
            while (player.choice == "")
                yield return null;
            for (int i = 0; i < blanktile.Count; i++)
                blanktile[i].choicescript.DisableButton();
            player.choicetext.transform.parent.gameObject.SetActive(false);

            this.pv.RPC("MovePath", RpcTarget.All, storedtile.position, player.chosentile.position);
        }
    }

    [PunRPC]
    void MovePath(int originalposition, int newposition)
    {
        TileData oldTile = Manager.instance.listoftiles[originalposition];
        TileData newTile = Manager.instance.listoftiles[newposition];
        Path chosenpath = oldTile.mypath;

        oldTile.mypath = null;
        chosenpath.NewHome(newTile);
    }
}
