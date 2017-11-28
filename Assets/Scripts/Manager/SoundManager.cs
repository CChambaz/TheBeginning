using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager sm_instance = null;
    public AudioSource musique_source;
    public AudioSource sounds_source;

    private void Awake()
    {
        if (sm_instance == null)
        {
            sm_instance = this;
        }
        else if (sm_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySounds(AudioClip clip)
    {
        sounds_source.clip = clip;
        sounds_source.Play();
    }
}
