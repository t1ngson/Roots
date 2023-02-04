using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneStart : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("temp");
    }
}
