using UnityEngine;
using System;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    private AudioSource[] audios;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound item in sounds)
        {
            item.source = gameObject.AddComponent<AudioSource>();
            item.source.clip = item.clip;

            item.source.volume = item.volume;
            item.source.pitch = item.pitch;
            item.source.loop = item.loop;

        }


        try
        {
            Play("Theme");
        }
        catch
        {

            Debug.Log("Theme not found");
        }

        audios = gameObject.GetComponents<AudioSource>();
        foreach (var item in audios)
        {
            Debug.Log(item.clip.name);
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }


    public void VolumeUpdate(float volume, bool isTheme)
    {
        //Debug.Log("Theme: " + isTheme + " Volume: " + volume);

        

        for (int i = 0; i < audios.Length; i++)
        {
            if (isTheme == true)
            {
                if (audios[i].clip.name == "Theme")
                    audios[i].volume = volume;
            }
            else if (audios[i].clip.name != "Theme")
                audios[i].volume = volume;

        }

    }

    public float GetAudioVolume(bool isTheme)
    {
        for (int i = 0; i < audios.Length; i++)
        {
            if (isTheme == true)
            {
                if (audios[i].clip.name == "Theme")
                    return audios[i].volume;
            }
            else if (audios[i].clip.name != "Theme")
                return audios[i].volume;
            

        }
        return 0f;
    }
}
