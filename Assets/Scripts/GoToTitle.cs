using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToTitle : MonoBehaviour
{
    private void Start()
    {
        if (AdjustRatio.instance == null)
            SceneManager.LoadScene(0);
    }
}
