using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.EventSystems;

public class Path : Card, IPointerClickHandler
{
    [ReadOnly] public bool flipped = false;

    //left, right, up, down

    [SerializeField] Sprite defaultimage;

    [SerializeField] bool[] enternormal = new bool[4];
    [SerializeField] bool[] enterflipped = new bool[4];

    [SerializeField] bool[] exitnormal = new bool[4];
    [SerializeField] bool[] exitflipped = new bool[4];

    [ReadOnly] public bool enterleft;
    [ReadOnly] public bool enterright;
    [ReadOnly] public bool enterup;
    [ReadOnly] public bool enterdown;

    [ReadOnly] public bool exitleft;
    [ReadOnly] public bool exitright;
    [ReadOnly] public bool exitup;
    [ReadOnly] public bool exitdown;

    private void Start()
    {
        this.myType = CardType.Path;

        enterleft = enternormal[0];
        enterright = enternormal[1];
        enterup = enternormal[2];
        enterdown = enternormal[3];

        exitleft = exitnormal[0];
        exitright = exitnormal[1];
        exitup = exitnormal[2];
        exitdown = exitnormal[3];
    }

    public void FlipCard(bool x)
    {
        if (x)
        {
            StartCoroutine(MoveCard(this.transform.localPosition, new Vector3(0, 0, 180), 0.3f));
            flipped = true;

            enterleft = enterflipped[0];
            enterright = enterflipped[1];
            enterup = enterflipped[2];
            enterdown = enterflipped[3];

            exitleft = exitflipped[0];
            exitright = exitflipped[1];
            exitup = exitflipped[2];
            exitdown = exitflipped[3];
        }
        else
        {
            StartCoroutine(MoveCard(this.transform.localPosition, new Vector3(0, 0, 0), 0.3f));
            flipped = false;

            enterleft = enternormal[0];
            enterright = enternormal[1];
            enterup = enternormal[2];
            enterdown = enternormal[3];

            exitleft = exitnormal[0];
            exitright = exitnormal[1];
            exitup = exitnormal[2];
            exitdown = exitnormal[3];
        }
    }

    public void FlipCard()
    {
        FlipCard(!flipped);
    }

    public void NewHome(TileData tile)
    {
        tile.mypath = this;
        this.transform.SetParent(this.transform);
        this.transform.SetSiblingIndex(0);
        StartCoroutine(this.MoveCard(new Vector2(0, 0), (flipped) ? new Vector3(0, 0, 180) : new Vector3(0, 0, 0), 0.3f));
    }

    protected override void SetSprite()
    {
        canvasgroup.alpha = 1;
        image.sprite = defaultimage;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BlowUpCard.instance.ChangeCard(this);
        }
    }
}
