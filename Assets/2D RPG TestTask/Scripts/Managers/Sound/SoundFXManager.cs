using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public static class SoundFXManager
{
    public enum Sound
    {
        PlayerAttack,
        PlayerMove,
        PlayerHurt,
        PlayerDie,
        EnemyHurt,
        EnemyDie,
        ButtonClick,
        BuyItem,
        NotEnoughGold,
        SellItem,
        PickupSword,
        PickupAxe,
        PickupCarrot,
        PickupCoin,
        Healing,
        Respawn,
        DropItem,
        AllLevelesCompleted
    }

    private const float SoundVolume = 0.3f;

    private static GameObject soundGameObject;
    private static Dictionary<Sound, float> soundTimerDictionary;
    private static List<AudioSource> audioSourcesPool;

    private static readonly HashSet<Sound> alwaysTrueSounds = new HashSet<Sound>
    {
        Sound.BuyItem,
        Sound.SellItem,
        Sound.Healing,
        Sound.EnemyHurt,
        Sound.PlayerHurt,
        Sound.PickupCoin
    };

    public static void Init()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();

        soundGameObject = new GameObject("Sound Manager");
        UnityEngine.Object.DontDestroyOnLoad(soundGameObject);

        audioSourcesPool = new List<AudioSource>();

        foreach (Sound sound in System.Enum.GetValues(typeof(Sound)))
        {
            soundTimerDictionary[sound] = 0f;
        }
    }

    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            AudioSource audioSource = GetAvailableAudioSource();
            if (audioSource != null)
            {
                AudioClip clip = GetAudioClip(sound);

                if (clip != null)
                {
                    audioSource.PlayOneShot(clip, SoundVolume);
                    Unity.VisualScripting.CoroutineRunner.instance.StartCoroutine(DestroyAudioSourceAfterDelay(audioSource, clip.length));
                }
            }
        }
    }

    private static IEnumerator DestroyAudioSourceAfterDelay(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        lock (audioSourcesPool)
        {
            if (audioSourcesPool.Contains(audioSource))
            {
                audioSourcesPool.Remove(audioSource);
                UnityEngine.Object.Destroy(audioSource);
            }
        }
    }

    private static bool CanPlayDefaultSound(Sound sound) //Prevent sound stack
    {
        if (soundTimerDictionary.TryGetValue(sound, out float lastTimePlayed))
        {
            float additionTimer = 0.01f;
            float timerMax = GetAudioClip(sound).length + additionTimer;

            bool canPlay = lastTimePlayed + timerMax < Time.time;
            if (canPlay)
            {
                soundTimerDictionary[sound] = Time.time;
            }

            return canPlay;
        }

        return true;
    }

    private static bool CanPlaySound(Sound sound)
    {
        return alwaysTrueSounds.Contains(sound) || CanPlayDefaultSound(sound);
    }

    private static AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource audioSource in audioSourcesPool)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }

        AudioSource newAudioSource = soundGameObject.AddComponent<AudioSource>();
        audioSourcesPool.Add(newAudioSource);
        return newAudioSource;
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (AudioHolder.SoundAudioClip soundAudioClip in AudioHolder.Instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }

    public static SoundFXManager.Sound GetPickupSound(Item item)
    {
        return item.itemType switch
        {
            ItemType.Sword => SoundFXManager.Sound.PickupSword,
            ItemType.Axe => SoundFXManager.Sound.PickupAxe,
            ItemType.Carrot => SoundFXManager.Sound.PickupCarrot,
            ItemType.Coin => SoundFXManager.Sound.PickupCoin,
            _ => throw new System.NotImplementedException()
        };
    }
}
