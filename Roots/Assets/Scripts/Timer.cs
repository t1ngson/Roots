using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeValue = 30;
    public Text timerText;

    public float deathTransitionTime = 3f;

    public Animator deathTransition;

    void Update()
    {
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
            LoadShopSceneDeath();
        }
        DisplayTime(timeValue);
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoadShopSceneDeath()
    {
        StartCoroutine(LoadDeathAnim());
    }

    IEnumerator LoadDeathAnim(){
        deathTransition.SetTrigger("StartDeath");
        yield return new WaitForSeconds(deathTransitionTime);
        SceneManager.LoadScene("Shop Scene");
    }



}
