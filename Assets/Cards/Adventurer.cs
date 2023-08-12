using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Adventurer : Explorer
{
    public override IEnumerator PlayThis(Player player)
    {
        player.AddMoves(2);

        for (int k = 0; k < 2; k++)
        {
            bool pathinhand = false;
            for (int i = 0; i < player.hand.childCount; i++)
            {
                SendChoice ct = player.hand.GetChild(i).GetComponent<SendChoice>();
                if (ct.card.path)
                {
                    ct.EnableButton(player);
                    pathinhand = true;
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
                for (int i = 0; i < player.hand.childCount; i++)
                    player.hand.GetChild(i).GetComponent<SendChoice>().DisableButton();
                player.choicetext.transform.parent.gameObject.SetActive(false);

                if (player.chosencard != null)
                {
                    Path cardforlater = player.chosencard.GetComponent<Path>();
                    for (int i = 0; i < Manager.instance.listoftiles.Count; i++)
                    {
                        TileData nexttile = Manager.instance.listoftiles[i];
                        if (!nexttile.river)
                            nexttile.choicescript.EnableButton(player);
                    }

                    player.choice = "";
                    player.chosentile = null;
                    while (player.choice == "")
                        yield return null;

                    for (int i = 0; i < Manager.instance.listoftiles.Count; i++)
                        Manager.instance.listoftiles[i].choicescript.DisableButton();

                    player.photonView.RPC("PathToForest", RpcTarget.All, cardforlater.pv.ViewID, player.chosentile.position, cardforlater.flipped);
                    player.photonView.RPC("CreatePathGrid", RpcTarget.All, cardforlater.pv.ViewID, cardforlater.flipped);
                }
            }
            else
                break;
        }
    }
}
