using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public string scene, thisScene;
    public GameObject exitPrompt;

    public void LoadScene(string scName) //ganti scene
    {
        SceneManager.LoadScene(scName);
    }

    public void Quit() //keluar aplikasi
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(thisScene == "Main Menu")
            {
                exitPrompt.SetActive(true);
            } else
            {
                SceneManager.LoadScene(scene);
            }
        }
    }
}
