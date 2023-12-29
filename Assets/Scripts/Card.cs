using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using MyBox;

public class Card : MonoBehaviour
{
    [ReadOnly] public PhotonView pv;
    public enum CardType { Path, Explorer};
    [ReadOnly] public CardType myType;
    [ReadOnly] public Image image;
    [ReadOnly] public SendChoice choicescript;
    protected CanvasGroup canvasgroup;

    void Awake()
    {
        pv = this.GetComponent<PhotonView>();
        image = GetComponent<Image>();
        canvasgroup = transform.Find("Canvas Group").GetComponent<CanvasGroup>();
        choicescript = GetComponent<SendChoice>();
    }

    public IEnumerator MoveCard(Vector2 newPos, Vector2 finalPos, Vector3 newRot, float waitTime)
    {
        float elapsedTime = 0;
        Vector2 originalPos = this.transform.localPosition;
        Vector3 originalRot = this.transform.localEulerAngles;

        while (elapsedTime < waitTime)
        {
            this.transform.localPosition = Vector2.Lerp(originalPos, newPos, elapsedTime / waitTime);
            this.transform.localEulerAngles = Vector3.Lerp(originalRot, newRot, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.transform.localPosition = finalPos;
    }

    public IEnumerator RevealCard(float totalTime)
    {
        if (image.sprite == Manager.instance.cardback)
        {
            canvasgroup.alpha = 0;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            float elapsedTime = 0f;

            Vector3 originalRot = this.transform.localEulerAngles;
            Vector3 newRot = new(0, 90, 0);

            while (elapsedTime < totalTime)
            {
                this.transform.localEulerAngles = Vector3.Lerp(originalRot, newRot, elapsedTime / totalTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            SetSprite();
            elapsedTime = 0f;

            while (elapsedTime < totalTime)
            {
                this.transform.localEulerAngles = Vector3.Lerp(newRot, originalRot, elapsedTime / totalTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            this.transform.localEulerAngles = originalRot;
        }
    }

    protected virtual void SetSprite()
    {
    }
}
