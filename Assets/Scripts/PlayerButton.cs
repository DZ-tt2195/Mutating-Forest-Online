using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerButton : MonoBehaviour
{
    [HideInInspector]public Pawn pawn;
    public TMP_Text textbox;
    public Button button;
    public Image image;
    Scrollbar scrollbar;

    public void Setup(Pawn pawn)
    {
        this.pawn = pawn;
        scrollbar = GameObject.Find("Forest Scrollbar").GetComponent<Scrollbar>();
    }

    public void MoveBar()
    {
        switch (pawn.currenttile.row)
        {
            case 0:
                scrollbar.value = 1;
                break;
            case 1:
                scrollbar.value = 0.93f;
                break;
            case 2:
                scrollbar.value = 0.8f;
                break;
            case 3:
                scrollbar.value = 0.65f;
                break;
            case 4:
                scrollbar.value = 0.5f;
                break;
            case 5:
                scrollbar.value = 0.35f;
                break;
            case 6:
                scrollbar.value = 0.2f;
                break;
            case 7:
                scrollbar.value = 0.05f;
                break;
            default:
                scrollbar.value = 0;
                break;
        }
    }

}
