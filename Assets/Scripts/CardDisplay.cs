using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image image;

    public void CardArt(Path path)
    {
        image.sprite = path.image.sprite;
    }
    public void CardArt(Explorer explorer)
    {
        image.sprite = explorer.image.sprite;
    }
}
