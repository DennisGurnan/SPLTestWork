using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager Singleton;
    // Start is called before the first frame update
    void Awake()
    {
        if (Singleton == null) Singleton = this;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Application.Quit(0);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
