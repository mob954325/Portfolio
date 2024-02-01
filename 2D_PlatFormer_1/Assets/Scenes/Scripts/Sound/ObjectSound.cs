using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSound : MonoBehaviour
{
    public AudioClip[] _audioClips;
    public AudioSource[] _audioSource;

    void Awake()
    {
        InitAudio();  
    }

    void InitAudio()
    {
        _audioSource = new AudioSource[_audioClips.Length];
        for(int i = 0; i < _audioClips.Length; i++)
        {
            _audioSource[i] = gameObject.AddComponent<AudioSource>();
            _audioSource[i].clip = _audioClips[i];
            _audioSource[i].playOnAwake = false;
            _audioSource[i].loop = false;
        }
    }
}
