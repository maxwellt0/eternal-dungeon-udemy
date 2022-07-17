using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] sfxList;
    public AudioSource[] musicList;
    public AudioSource menuMusic;

    public void PlaySfx(int index)
    {
        sfxList[index].Stop();
        sfxList[index].pitch = Random.Range(0.9f, 1.1f);
        sfxList[index].Play();
    }

    public void PlayRandomMusic()
    {
        musicList[Random.Range(0, musicList.Length)].Play();
    }

    public void PlayMenuMusic()
    {
        menuMusic.Play();
    }
}
