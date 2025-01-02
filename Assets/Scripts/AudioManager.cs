using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance { get; private set; }  // Singleton instance
    private void Awake()
    {
        // Ensure that only one instance of UiManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destroy any duplicate UiManager
        }
    }
    #endregion

    AudioSource audioSource;

    public AudioClip bgm;

    private void Start()
    {
        init();
    }

    void init()
    {
        audioSource = GetComponent<AudioSource>();
        SoundToggle();
    }

    public void SoundToggle()
    {
        bool isSoundOn = Prefs.SoundToggle > 0;
        if (isSoundOn)
        {
            UiManager.Instance.SetSoundActive(true);
            audioSource.mute = false;
            if (!audioSource.isPlaying) 
                audioSource.Play();
            Prefs.SoundToggle = 0;
        }
        else 
        {
            audioSource.Pause();
            UiManager.Instance.SetSoundActive(false);
            audioSource.mute = true;
            Prefs.SoundToggle = 1;
        }
    }
}
