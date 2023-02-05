using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{

    public AudioSource adSource;
    public AudioSource deathSound;
    public AudioSource splash;
    public AudioSource sparkle;
    public AudioSource whack;
    public AudioSource nom;

    void Start()
    {
        //adSource.Play();   
    }

    // Update is called once per frame
    void Update()
    {

    }

    void playSound(AudioSource audioSource) {
        audioSource.Play();
    }

    public void playDeathSound()
    {
        deathSound.Play();
    }

    public void playSplash()
    {
        splash.Play();
    }

    public void playSparkle()
    {
        sparkle.Play();
    }

    public void playWhack()
    {
        whack.Play();
    }

    public void playNomNom()
    {
        nom.Play();
    }
}
