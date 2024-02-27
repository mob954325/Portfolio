using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� �ҽ��� ����ִ� Ŭ���� 
/// </summary>
public class SoundControl : MonoBehaviour
{
    public AudioSource[] audioSources;

    void Awake()
    {
        audioSources = new AudioSource[transform.childCount];

        for(int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = transform.GetChild(i).GetComponent<AudioSource>();
        }
    }
}
