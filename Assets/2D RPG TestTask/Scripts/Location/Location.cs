using UnityEngine;

public class Location : MonoBehaviour
{
    [Header("Location Settings")]
    [SerializeField] private bool isFightLocation;

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isPlayer = collision.CompareTag("Player");

        if (isPlayer)
        {
            BackgroundMusicManager.Instance.PlayLocationMusic(isFightLocation);
        }
    }
}
