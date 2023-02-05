using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneStart : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public void PlayButton()
    {
        LoadNextScene("GameScene");
    }

    public void GoToEndScene(){
        LoadNextScene("End Scene");
    }

    public void GoToShop(){
        LoadNextScene("Shop Scene");
    }

    public void GoToTutorial(){
        LoadNextScene("Tutorial Scene");
    }

    public void GoToMenu(){
        LoadNextScene("Main Menu");
    }


    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadLevelAnim(sceneName));
    }

    IEnumerator LoadLevelAnim(string sceneName)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
