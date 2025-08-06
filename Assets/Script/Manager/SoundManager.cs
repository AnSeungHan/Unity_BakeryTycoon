using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager 
    : Singleton<SoundManager>
{
    enum SoundType
    {
        BGM,
        SFX
    }

    [System.Serializable]
    public struct SoundInfo
    {
        public string      name;
        public AudioClip   audio;
    }

    [SerializeField]
    private SoundInfo[] bgmSound;
    [SerializeField]
    private SoundInfo[] sfxSound;

    private Dictionary<string, AudioClip> bgmAudioClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxAudioClips = new Dictionary<string, AudioClip>();
    private AudioSource                   bgmPlayer;
    private AudioSource[]                 sfxPlayer;

    protected new void Awake()
    {
        base.Awake();

        Transform bgm = transform.Find("BGM");
        Transform sfx = transform.Find("SFX");

        if (bgm)
        {
            AudioSource audioSource = bgm.GetComponent<AudioSource>();
            bgmPlayer = new AudioSource();
            bgmPlayer = bgm.GetComponent<AudioSource>();
        }

        if (sfx)
        {
            AudioSource[] audioSources = bgm.GetComponents<AudioSource>();
            sfxPlayer = new AudioSource[audioSources.Length];
            sfxPlayer = sfx.GetComponents<AudioSource>();
        }

        foreach (SoundInfo elem in bgmSound)
            bgmAudioClips.Add(elem.name, elem.audio);

        foreach (SoundInfo elem in sfxSound)
            sfxAudioClips.Add(elem.name, elem.audio);
    }

    public void PlayBGM(string _SoundName)
    {
        if (!bgmAudioClips.ContainsKey(_SoundName))
            return;

        bgmPlayer.clip = bgmAudioClips[_SoundName];
        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(string _SoundName)
    {
        if (!sfxAudioClips.ContainsKey(_SoundName))
            return;

        for (int i = 0; i < sfxPlayer.Length; ++i)
        {
            if (sfxPlayer[i].isPlaying)
                continue;

            sfxPlayer[i].clip = sfxAudioClips[_SoundName];
            sfxPlayer[i].Play();

            return;
        }

        Debug.Log("All SFX Use");
    }

    private void FixedUpdate()
    {
        if (!bgmPlayer.isPlaying)
            PlayBGM("MainBGM");
    }
}
