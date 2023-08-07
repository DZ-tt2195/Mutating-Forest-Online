using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileData : MonoBehaviour
{
    [HideInInspector] public int row;
    [HideInInspector] public int column;
    public int position;
    public Sprite blankart;
    public Sprite riverart;
    [HideInInspector] public bool river = false;

    public Path mypath;

    [HideInInspector] public TileData up;
    [HideInInspector] public TileData left;
    [HideInInspector] public TileData down;
    [HideInInspector] public TileData right;

    public SendChoice choicescript;
    public Image image;

    // Start is called before the first frame update
    void Start()
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
