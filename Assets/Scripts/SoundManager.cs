using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip scoreSound;
    public AudioClip dieSound;
    public AudioClip gameBGM;

    private void Start()
    {
        GameObject bgm = new GameObject("BGM");
        AudioSource bgmSource = bgm.AddComponent<AudioSource>();
        bgmSource.clip = gameBGM;
        bgmSource.volume = 0.10f;
        bgmSource.priority = 128;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayScoreSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(scoreSound);
    }

    public void PlayDieSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(dieSound);
    }
}
