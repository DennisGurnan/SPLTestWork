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
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void ShowVictory()
    {
        victoryFrame.SetActive(true);
        if (GameManager.Singleton.victorySound != null) GameManager.Singleton.victorySound.Play();
        Cursor.visible = true;
    }

    public void ShowDefard()
    {
        defardFrame.SetActive(true);
        if (GameManager.Singleton.defardSound != null) GameManager.Singleton.defardSound.Play();
        Cursor.visible = true;
    }
}
