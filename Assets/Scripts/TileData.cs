using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class TileData : MonoBehaviour
{
    [ReadOnly] public int row;
    [ReadOnly] public int column;
    [ReadOnly] public int position;
    [SerializeField] Sprite blankart;
    [SerializeField] Sprite riverart;
    [ReadOnly] public bool river = false;

    [ReadOnly] public Path mypath;

    [ReadOnly] public TileData up;
    [ReadOnly] public TileData left;
    [ReadOnly] public TileData down;
    [ReadOnly] public TileData right;

    [ReadOnly] public SendChoice choicescript;
    [ReadOnly] Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        choicescript = GetComponent<SendChoice>();
    }

    public void SetRiver()
    {
        river = true;
        image.sprite = riverart;
    }

    public void NewTile(Path newpath)
    {
        if (newpath == null)
        {
            mypath = null;
            image.sprite = blankart;
        }
        else
        {
            mypath = newpath;
            image.sprite = newpath.image.sprite;
        }
    }

    public List<Pawn> PlayersOnThis()
    {
        List<Pawn> playersonthis = new List<Pawn>();

        for (int i = 0; i<this.transform.childCount; i++)
        {
            Pawn isPlayer = transform.GetChild(i).GetComponent<Pawn>();
            if (isPlayer != null)
                playersonthis.Add(isPlayer);
        }

        return playersonthis;
    }
}
