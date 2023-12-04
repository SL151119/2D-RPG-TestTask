using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("Location Music Settings")]
    [SerializeField] private AudioClip shopLocation;
    [SerializeField] private AudioClip fightLocation;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void PlayMusic(AudioClip clip)
    {
        if (CanPlayMusic(clip))
        {
            audioSource.Play();
            audioSource.clip = clip;
        }
    }

    public bool CanPlayMusic(AudioClip clip) => audioSource.clip != clip || !audioSource.isPlaying;

    public void PlayLocationMusic(bool isFightLocation) => PlayMusic(isFightLocation ? fightLocation : shopLocation);
}
