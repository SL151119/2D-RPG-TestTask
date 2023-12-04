using System;
using UnityEngine;

public class AudioHolder : MonoBehaviour
{
    [Header("Audio Holder Settings")]
    public SoundAudioClip[] soundAudioClipArray;

    public static AudioHolder Instance { get; private set; }

    private void Awake() 
    { 
        if (Instance == null) 
        { 
            Instance = this; 
        } 
    }

    [Serializable]
    public class SoundAudioClip
    {
        public SoundFXManager.Sound sound;
        public AudioClip audioClip;
    }
}
