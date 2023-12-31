using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] Image blownUp;
    [SerializeField] CanvasGroup canvasgroup;
    [SerializeField] Image artwork;
    [SerializeField] TMP_Text cardName;
    [SerializeField] TMP_Text cardText;
    [SerializeField] TMP_Text cardArtist;

    public void CardArt(Path path)
    {
        blownUp.transform.localEulerAngles = path.transform.localEulerAngles;
        blownUp.sprite = path.image.sprite;
        canvasgroup.alpha = 0;
    }
    public void CardArt(Explorer explorer)
    {
        blownUp.transform.localEulerAngles = new Vector3(0, 0, 0);
        artwork.sprite = explorer.artwork.sprite;
        cardName.text = explorer.cardName.text;
        cardText.text = explorer.cardText.text;
        cardArtist.text = explorer.cardArtist.text;
        canvasgroup.alpha = 1;
    }
}
