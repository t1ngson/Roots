using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneStart : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToEndScene(){
        SceneManager.LoadScene("End Scene");
    }

    public void GoToShop(){
        SceneManager.LoadScene("Shop Scene");
    }
}
