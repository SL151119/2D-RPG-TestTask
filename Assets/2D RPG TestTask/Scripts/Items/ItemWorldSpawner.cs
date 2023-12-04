using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour
{
    [Header("ItemToSpawn")]
    [SerializeField] private Item item;

    private void Start()
    {
        SpawnItemAndDestroy();
    }

    private void SpawnItemAndDestroy()
    {
        ItemWorld.SpawnItemWorld(transform.position, item);
        Destroy(gameObject);
    }
}
