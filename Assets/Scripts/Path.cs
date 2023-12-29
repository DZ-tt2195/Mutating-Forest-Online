using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Path : Card
{
    [HideInInspector] public bool flipped = false;

    //left, right, up, down

    [SerializeField] Sprite defaultimage;
    [SerializeField] Sprite flippedimage;

    [SerializeField] bool[] enternormal = new bool[4];
    [SerializeField] bool[] enterflipped = new bool[4];

    [SerializeField] bool[] exitnormal = new bool[4];
    [SerializeField] bool[] exitflipped = new bool[4];

    [SerializeField] public bool enterleft;
    [SerializeField] public bool enterright;
    [SerializeField] public bool enterup;
    [SerializeField] public bool enterdown;

    [SerializeField] public bool exitleft;
    [SerializeField] public bool exitright;
    [SerializeField] public bool exitup;
    [SerializeField] public bool exitdown;

    private void Awake()
    {
        this.myType = CardType.Explorer;

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
            this.image.sprite = flippedimage;
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
            this.image.sprite = defaultimage;
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
}
