using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    [Header("Audio shit")]
    public static SFXManager instance;
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    private HashMap<string, AudioClip> audioStuff;

    private AudioSource noLoopsfx;// for non background track

    private AudioSource loopSfx;// for background track
    
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        noLoopsfx = gameObject.AddComponent<AudioSource>();
        noLoopsfx.outputAudioMixerGroup = sfxMixerGroup;
        loopSfx = gameObject.AddComponent<AudioSource>();
        noLoopsfx.volume = 1f;
        loopSfx.loop = true;
        loopSfx.volume = 0.4f;
        loopSfx.outputAudioMixerGroup = musicMixerGroup;
        audioStuff = new HashMap<string, AudioClip>();
        AddtoHASSSH();
    }
    private void AddtoHASSSH()
    {
        foreach (AudioClip clip in audioClips)
        {
            if (clip == null)
            {
                Debug.LogWarning("Daar is Niks"); // safety check
            } 
            audioStuff.Add(clip.name, clip);// uses the name as the key
        }
    }

    public void PlayRequestedSound(string clipName, bool isLoop = false)
    {// this method would be called wherever, it will take the string, search the hashMap and then if its in the hashmap it will play
        AudioClip clip = audioStuff.Find(clipName);
        if (isLoop)
        {
            if (loopSfx.clip == clip && loopSfx.isPlaying)
            {
                Debug.LogWarning("Naur this thing is already playing");
                return; // if it is playing already, it will return
            }
            
            loopSfx.clip = clip;
            loopSfx.loop = true;
            loopSfx.Play();
        }
        else  
        {
            if (noLoopsfx.clip == clip && noLoopsfx.isPlaying)
            {
                Debug.LogWarning("Naur this thing is already playing");
                return;
            }
            noLoopsfx.PlayOneShot(clip);
        }
        
    }
    public void StopSound()
    {
        loopSfx.Stop();
        noLoopsfx.Stop();
    }
}
