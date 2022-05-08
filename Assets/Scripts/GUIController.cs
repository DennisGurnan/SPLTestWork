using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    public static GUIController Singleton;
    public GameObject victoryFrame;
    public GameObject defardFrame;
    // Start is called before the first frame update
    void Start()
    {
        if (Singleton == null) Singleton = this;
        victoryFrame.SetActive(false);
        defardFrame.SetActive(false);
    }

    public void ShowVictory()
    {
        victoryFrame.SetActive(true);
    }

    public void ShowDefard()
    {
        defardFrame.SetActive(true);
    }
}
