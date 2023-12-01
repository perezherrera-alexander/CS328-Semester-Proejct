using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds;
    public AudioSource musicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        string activeScene = SceneManager.GetActiveScene().name;

        if (activeScene == "MainMenu" || activeScene == "LevelSelect")
        {
            PlayMusic("MenuMusic");
        }
        else
        {
            PlayMusic("GameMusic");
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, sound => sound.soundName == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }


}
