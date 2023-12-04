using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    public Transform itemWorldPrefab;

    public Sprite swordSprite;
    public Sprite axeSprite;
    public Sprite carrotSprite;
    public Sprite coinSprite;

    public GameObject chestPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
