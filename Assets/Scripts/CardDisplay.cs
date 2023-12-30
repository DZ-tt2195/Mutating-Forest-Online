using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image image;

    public void CardArt(Path path, bool flipped)
    {
        image.sprite = path.image.sprite;
        this.transform.localEulerAngles = (flipped) ? new Vector3(0, 0, 180) : new Vector3(0, 0, 0);
    }
    public void CardArt(Explorer explorer)
    {
        image.sprite = explorer.image.sprite;
    }
}
