using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using MyBox;

public class Pawn : MonoBehaviour
{
    [ReadOnly] public PhotonView pv;
    [ReadOnly] public Image image;
    [ReadOnly] public PhotonView flag;

    [ReadOnly] public Player controller;
    [ReadOnly] public TileData currenttile;
    [ReadOnly] public TileData endtile;

    public void SetStart(Player controller, int position)
    {
        this.controller = controller;
        pv.RPC("NewPosition", RpcTarget.All, position);
        switch (currenttile.position)
        {
            case (0):
                endtile = Manager.instance.listoftiles[44];
                break;
            case (44):
                endtile = Manager.instance.listoftiles[0];
                break;
            case (40):
                endtile = Manager.instance.listoftiles[4];
                break;
            case (4):
                endtile = Manager.instance.listoftiles[40];
                break;
        }
        pv.RPC("SetFlag", RpcTarget.All, endtile.position);
    }

    [PunRPC]
    void SetFlag(int position)
    {
        flag.transform.SetParent(Manager.instance.listoftiles[position].transform);
        flag.transform.SetSiblingIndex(2);
        flag.GetComponent<RectTransform>().localPosition = new Vector2(0, 115);
        flag.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 115);
    }

    [PunRPC]
    void NewPosition(int newposition)
    {
        currenttile = Manager.instance.listoftiles[newposition];
        this.transform.SetParent(currenttile.transform);
        this.transform.SetSiblingIndex(2);
        this.transform.localPosition = new Vector3(0, 0, 0);

        if (endtile != null && currenttile.position == endtile.position)
            Manager.instance.YouWon(controller.name);
    }

    public void MoveOn(TileData nexttile, bool druid)
    {
        TileData previoustile = currenttile;
        pv.RPC("NewPosition", RpcTarget.All, nexttile.position);

        if (druid && previoustile.mypath != null)
            controller.photonView.RPC("PathToForest", RpcTarget.All, -1, previoustile.position, false);
    }

    public TileData CanMove()
    {
        if (this.currenttile.mypath == null)
            return null;

        if (this.currenttile.mypath.exitleft)
        {
            if (currenttile.left == null || currenttile.left.mypath == null)
                return null;
            return (currenttile.left);
        }
        else if (this.currenttile.mypath.exitright)
        {
            if (currenttile.right == null || currenttile.right.mypath == null)
                return null;
            return (currenttile.right);
        }
        else if (this.currenttile.mypath.exitup)
        {
            if (currenttile.up == null || currenttile.up.mypath == null)
                return null;
            return (currenttile.up);
        }
        else if (this.currenttile.mypath.exitdown)
        {
            if (currenttile.down == null || currenttile.down.mypath == null)
                return null;
            return (currenttile.down);
        }
        return null;
    }
}
