using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustRatio : MonoBehaviour
{
    public static AdjustRatio instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        /*
        float targetRatio = (float) 16 / 9;
        int newWidth = Mathf.RoundToInt(Screen.height * targetRatio);
        int newHeight = Mathf.RoundToInt(Screen.width / targetRatio);

        if (newWidth > Screen.width)
            newWidth = Screen.width;
        else
            newHeight = Screen.height;

        Screen.SetResolution(newWidth, newHeight, false);
        */
    }
}
