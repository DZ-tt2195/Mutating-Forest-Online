using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlowUpCard : MonoBehaviour
{
    public static BlowUpCard instance;
    [SerializeField] Image blownUp;
    [SerializeField] CanvasGroup canvasgroup;
    [SerializeField] Image artwork;
    [SerializeField] TMP_Text cardName;
    [SerializeField] TMP_Text cardText;
    [SerializeField] TMP_Text cardArtist;

    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            blownUp.transform.parent.gameObject.SetActive(false);
    }

    public void ChangeCard(Explorer explorer)
    {
        blownUp.transform.parent.gameObject.SetActive(true);
        blownUp.transform.localEulerAngles = new Vector3(0, 0, 0);
        artwork.sprite = explorer.image.sprite;
        cardName.text = explorer.cardName.text;
        cardText.text = explorer.cardText.text;
        cardArtist.text = explorer.cardArtist.text;
        canvasgroup.alpha = 1;
    }

    public void ChangeCard(Path path)
    {
        blownUp.transform.parent.gameObject.SetActive(true);
        blownUp.transform.localEulerAngles = path.transform.localEulerAngles;
        blownUp.sprite = path.image.sprite;
        canvasgroup.alpha = 0;
    }
}
