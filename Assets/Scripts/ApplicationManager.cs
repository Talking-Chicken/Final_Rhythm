using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;

public class ApplicationManager : MMSingleton<ApplicationManager>
{
    void Start() {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void moveToStart() {
        SceneManager.LoadScene("Main");
    }
}
