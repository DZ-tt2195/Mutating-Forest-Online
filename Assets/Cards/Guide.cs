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
        this.pv.RPC("GuideChoice", prevPlayer.photonrealtime, prevPlayer.position, player.position);

        stillResolving = true;
        while (stillResolving)
            yield return null;
    }

    [PunRPC]
    IEnumerator GuideChoice(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerordergameobject[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerordergameobject[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, thisPlayer.name);

        thisPlayer.choicetext.transform.parent.gameObject.SetActive(true);
        thisPlayer.choicetext.text = $"{this.name}: Choose a bonus for {requestingPlayer.name}";

        List<SendChoice> listofchoices = new List<SendChoice>{ thisPlayer.CreateButton("+2 Draw"), thisPlayer.CreateButton("+2 Plays"), thisPlayer.CreateButton("+2 Moves")};

        thisPlayer.choice = "";
        while (thisPlayer.choice == "")
            yield return null;

        for (int i = 0; i < listofchoices.Count; i++)
            Destroy(listofchoices[i].gameObject);

        thisPlayer.choicetext.transform.parent.gameObject.SetActive(false);
        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerorderphotonlist[requestingPlayerPosition]);
        this.pv.RPC("ExecuteChoice", requestingPlayer.photonrealtime, requestingPlayer.position, thisPlayer.choice);
    }

    [PunRPC]
    void ExecuteChoice(int currPlayerPosition, string choice)
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
    void Finished()
    {
        stillResolving = false;
    }
}
