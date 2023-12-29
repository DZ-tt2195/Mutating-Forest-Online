using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Guide : Explorer
{
    bool stillResolving = true;

    protected override void GetText()
    {
        this.name = "Guide";
        cardName.text = "Guide";
        cardText.text = "+1 Play\nThe previous player chooses one for you: +2 Draw; or +2 Play; or +2 Move.";
        cardArtist.text = "Eric J Carter (“Guide”)";
    }

    public override IEnumerator PlayThis(Player player)
    {
        player.AddPlays(1);
        Player prevPlayer = player.GetPreviousPlayer();

        if (prevPlayer != null)
            prevPlayer = player;

        this.pv.RPC("GuideChoice", prevPlayer.photonrealtime, player.photonrealtime);

        stillResolving = true; ;
        while (stillResolving)
            yield return null;
    }

    [PunRPC]
    public IEnumerator GuideChoice(int prevPlayerPosition, int currPlayerPosition)
    {
        Player currPlayer = Manager.instance.playerordergameobject[currPlayerPosition];
        Player prevPlayer = Manager.instance.playerordergameobject[prevPlayerPosition];
        prevPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, prevPlayer.name);

        prevPlayer.choicetext.transform.parent.gameObject.SetActive(true);
        prevPlayer.choicetext.text = $"{this.name}: Choose a bonus for {currPlayer.name}";

        List<SendChoice> listofchoices = new List<SendChoice>{prevPlayer.CreateButton("+2 Draw"), prevPlayer.CreateButton("+2 Plays"), prevPlayer.CreateButton("+2 Moves")};

        prevPlayer.choice = "";
        while (prevPlayer.choice == "")
            yield return null;

        for (int i = 0; i < listofchoices.Count; i++)
            Destroy(listofchoices[i].gameObject);

        prevPlayer.choicetext.transform.parent.gameObject.SetActive(false);
        this.pv.RPC("ExecuteChoice", currPlayer.photonrealtime, currPlayerPosition, prevPlayer.choice);
    }

    [PunRPC]
    public void ExecuteChoice(int currPlayerPosition, string choice)
    {
        Player currPlayer = Manager.instance.playerordergameobject[currPlayerPosition];

        switch (choice)
        {
            case "+2 Draw":
                currPlayer.DrawCardRPC(2);
                break;
            case "+2 Plays":
                currPlayer.AddPlays(2);
                break;
            case "+2 Moves":
                currPlayer.AddMoves(2);
                break;
            default:
                break;
        }
        currPlayer.pv.RPC("WaitDone", Manager.instance.playerorderphotonlist[currPlayerPosition]);
        pv.RPC("Finished", RpcTarget.All);
    }

    [PunRPC]
    public void Finished()
    {
        stillResolving = false;
    }
}
