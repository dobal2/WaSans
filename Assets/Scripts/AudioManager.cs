using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        Init();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 이미 존재하면 새 인스턴스 제거
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
    }
    
    
    [Header("#BGM")]
    public AudioSource bgmPlayer;
    public AudioClip[] bgmClips;
    public float bgmVolume;
    private int _bgmChannelIndex;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;

    
    [Header("#SFX")]
    public AudioSource[] sfxPlayers;
    public AudioClip[] sfxClips;
    public int sfxChannelCount;
    public float sfxVolume;
    private int _sfxChannelIndex;
    //[SerializeField] private AudioMixerGroup sfxMixerGroup;
    
    public enum Bgm{FightBGM,GameOveBGM,GameClearBGM}
    public enum Sfx{GasterBlasterAppear,GasterBlasterShoot,SansTeleport,StrikeSans,BoneWarning}

    private void Start()
    {
        PlayBgm(Bgm.FightBGM);
    }

    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.outputAudioMixerGroup = bgmMixerGroup;

        GameObject sfxObject = new GameObject("SfxPlayerGroup");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[sfxChannelCount];

        for (int i = 0; i < sfxPlayers.Length;i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
            //sfxPlayers[i].outputAudioMixerGroup = sfxMixerGroup;
        }

    }
    

    public void PlayBgm(Bgm bgm)
    {
        bgmPlayer.clip = bgmClips[(int)bgm];
        bgmPlayer.Play();
    }
    
    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + _sfxChannelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            _sfxChannelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
        
    }

    public void PlayBgm()
    {
        bgmPlayer.Play();
    }

    public void StopBgm()
    {
        bgmPlayer.Stop();
    }
    
    public void PlaySfx()
    {
        foreach (var sfxPlayer in sfxPlayers)
        {
            sfxPlayer.Play();
            sfxPlayer.volume = sfxVolume;
        }
    }

    public void StopSfx()
    {
        foreach (var sfxPlayer in sfxPlayers)
        {
            sfxPlayer.Stop();
            sfxPlayer.clip = null;
            sfxPlayer.volume = 0;
        }
    }
    

    public void ChangeBgm(bool isOn)
    {
        if (isOn)
        {
            PlayBgm();
        }
        else
        {
            StopBgm();
        }
    }
    
    public void ChangeSfx(bool isOn)
    {
        Debug.Log(isOn);
        if (isOn)
        {
            PlaySfx();
        }
        else
        {
            StopSfx();
        }
    }
    
    
}