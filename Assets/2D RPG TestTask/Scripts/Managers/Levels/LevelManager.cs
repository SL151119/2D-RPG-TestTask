using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Path Collider")]
    [SerializeField] private BoxCollider2D pathCollider;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI enemiesAmout;

    private void OnEnable() => EnemiesSpawner.OnAllEnemiesDead += EnemiesSpawner_OnAllEnemiesDead; 

    private void Start()
    {
        pathCollider.isTrigger = false;
        currentLevel.text = ($"{Constants.LEVEL} {SceneLoadManager.Instance.LevelIndex + 1}");
    }

    private void Update()
    {
        enemiesAmout.text = ($"{Constants.ENEMIES_AMOUNT} {EnemiesSpawner.TotalEnemiesAlive}");
    }

    private void EnemiesSpawner_OnAllEnemiesDead()
    {
        if (SceneLoadManager.Instance.IsLastLevel())
        {
            SoundFXManager.PlaySound(SoundFXManager.Sound.AllLevelesCompleted);

            this.UniversalWait(0.5f, () =>
                Instantiate(ItemAssets.Instance.chestPrefab, GameManager.Instance.GetRandomSpawnPosition(), Quaternion.identity)    //Spawn chest
            );

            SceneLoadManager.Instance.ResetLevelIndex();
        }
        else
        {
            SceneLoadManager.Instance.IncreaseLevelIndex();
        }

        pathCollider.isTrigger = true;
    }

    private void OnDisable() => EnemiesSpawner.OnAllEnemiesDead -= EnemiesSpawner_OnAllEnemiesDead;
}
