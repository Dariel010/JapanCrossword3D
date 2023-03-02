using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioSource> _musicAudioSources;
    [SerializeField] private List<AudioSource> _clicksAudioSources;
    private int indexIsPlaying;
    private float crossFadeRate = 0.1f;
    public bool isCrossRoomActive, crossFading = false;
    public static AudioPlayer instance;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        isCrossRoomActive = false;
        indexIsPlaying = 0;
        _musicAudioSources[indexIsPlaying].Play();
    }

    void Update()
    {
        if (crossFading == false)
        {
            if (isCrossRoomActive)
            {
                if (indexIsPlaying == 0)
                {
                    StartCoroutine(CrossFadeMusic(_musicAudioSources[0], _musicAudioSources[1], 1));
                    Debug.Log("AUDIO>play(" + indexIsPlaying + ")="+ _musicAudioSources[1].clip.name);
                }
                else
                {
                    ActiveMusicRotation();
                }
            }
            else if (indexIsPlaying != 0)
            {
                StartCoroutine(CrossFadeMusic(_musicAudioSources[indexIsPlaying], _musicAudioSources[0], 0));
            }
        }
    }

    public void PlayClick(int index)
    {
        _clicksAudioSources[index].PlayOneShot(_clicksAudioSources[index].clip);
    }

    public void PlayLongSound(int index, bool isPlay)
    {
        if (isPlay)
        {
            if (!_clicksAudioSources[index].isPlaying)
            {
                _clicksAudioSources[index].Play();
            }
        }
        else
        {
            _clicksAudioSources[index].Stop();
        }
    }

    private void BackToIntro()
    {
        if (_musicAudioSources[indexIsPlaying].clip.length == _musicAudioSources[indexIsPlaying].time)
        {
            _musicAudioSources[indexIsPlaying].Stop();
            indexIsPlaying = 0;
            _musicAudioSources[indexIsPlaying].Play();
        }
    }

    private void ActiveMusicRotation()
    {
        if(_musicAudioSources[indexIsPlaying].clip.length <= _musicAudioSources[indexIsPlaying].time + 10)
        {
            Debug.Log("_musicAudioSources[" + indexIsPlaying + "].clip.length=" + _musicAudioSources[indexIsPlaying].clip.length);
            Debug.Log("_musicAudioSources[" + indexIsPlaying + "].time + 5=" + _musicAudioSources[indexIsPlaying].time + 10);
            //_musicAudioSources[indexIsPlaying].Stop();
            indexIsPlaying += 1;
            if (indexIsPlaying > _musicAudioSources.Count-1)
            {
                indexIsPlaying = 1;
            }
            //_musicAudioSources[indexIsPlaying].Play();
            StartCoroutine(CrossFadeMusic(_musicAudioSources[indexIsPlaying - 1], _musicAudioSources[indexIsPlaying], indexIsPlaying));
            Debug.Log("AUDIO>ActiveMusicRotation> play(" + indexIsPlaying + ")=" + _musicAudioSources[indexIsPlaying].clip.name);
        }
    }

    private IEnumerator CrossFadeMusic(AudioSource origin, AudioSource next, int indexNowPlaying)
    {
        crossFading = true;
        next.volume = 0;
        next.Play();
        indexIsPlaying = indexNowPlaying;
        float musicVolum = origin.volume;
        float scaledRate = crossFadeRate * musicVolum;
        Debug.Log("AUDIO>START CrossFadeMusic");
        while (origin.volume > 0)
        {
            origin.volume -= scaledRate * Time.deltaTime;
            next.volume += scaledRate * Time.deltaTime;
            yield return null; 
        }
        next.volume = musicVolum;  
        origin.Stop();
        crossFading = false;
        Debug.Log("AUDIO>End CrossFadeMusic");
    }
}
