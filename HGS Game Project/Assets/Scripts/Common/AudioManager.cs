using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 오디오 관리자 인스턴스

    public AudioSource musicSource; // 배경 음악을 재생할 오디오 소스

    private void Awake()
    {
        // Check if another instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set reference to musicSource
        musicSource = GetComponent<AudioSource>();
    }


    // 배경 음악 재생 함수
    public void PlayMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    // 배경 음악 정지 함수
    public void StopMusic()
    {
        musicSource.Stop();
    }
}
