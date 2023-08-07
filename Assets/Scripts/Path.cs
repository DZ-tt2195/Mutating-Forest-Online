using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Path : Card
{
    [HideInInspector] public bool flipped = false;

    //left, right, up, down

    public Sprite defaultimage;
    public Sprite flippedimage;

    public bool[] enternormal = new bool[4];
    public bool[] enterflipped = new bool[4];

    public bool[] exitnormal = new bool[4];
    public bool[] exitflipped = new bool[4];

    [HideInInspector] public bool enterleft;
    [HideInInspector] public bool enterright;
    [HideInInspector] public bool enterup;
    [HideInInspector] public bool enterdown;

    [HideInInspector] public bool exitleft;
    [HideInInspector] public bool exitright;
    [HideInInspector] public bool exitup;
    [HideInInspector] public bool exitdown;

    private void Awake()
    {
        this.path = true;

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
