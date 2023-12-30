using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerClickHandler
{
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            /*
            Manager.instance.blownUp.sprite = image.sprite;
            Manager.instance.blownUp.transform.parent.gameObject.SetActive(true);
            Manager.instance.blownUp.transform.parent.SetAsLastSibling();
            */
        }
    }
}
