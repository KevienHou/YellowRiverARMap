using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneController : MonoBehaviour
{
    public void LoadScene(string SceneName) 
    {
        SceneManager.LoadScene(SceneName);
    }


    public void QuitScene() 
    {
        Application.Quit();
    }

}
