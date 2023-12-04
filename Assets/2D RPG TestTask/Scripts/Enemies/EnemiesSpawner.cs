using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private float spawnRadius;
    [SerializeField] private Transform centerFirstCircle;
    [SerializeField] private Transform centerSecondCircle;

    [Serializable]
    class Wave
    {
        [Header("Wave Settings")]
        public float spawnTime;
        public Enemy enemyPrefab;
        public int countMin, countMax;
    }

    [SerializeField] private Wave[] waves;

    private int waveIndex;
    private float levelStartTime;

    private Wave CurrentWave => waves[waveIndex];
    private bool HasWaves => waveIndex < waves.Length;

    private int[] TotalEnemiesToSpawn;

    private readonly List<Enemy> SpawnedEnemies = new List<Enemy>();

    public static int TotalEnemiesAlive { get; private set; }

    public static Action OnAllEnemiesDead;

    private void Start()
    {
        levelStartTime = Time.timeSinceLevelLoad;
        InitializeTotalEnemyCount();
        UpdateTotalEnemiesAlive();
    }

    private void Update()
    {
        if (HasWaves && ShouldSpawnNextWave())
        {
            Spawn();
            waveIndex++;
        }
    }

    private void InitializeTotalEnemyCount()
    {
        TotalEnemiesToSpawn = new int[waves.Length];

        for (int i = 0; i < waves.Length; i++)
        {
            TotalEnemiesToSpawn[i] = UnityEngine.Random.Range(waves[i].countMin, waves[i].countMax + 1);
        }
    }

    private void UpdateTotalEnemiesAlive()
    {
        TotalEnemiesAlive = TotalEnemiesToSpawn.Sum();
    }

    private bool ShouldSpawnNextWave() => (Time.timeSinceLevelLoad - levelStartTime) > CurrentWave.spawnTime;

    private void Spawn()
    {
        int count = TotalEnemiesToSpawn[waveIndex];

        for (int i = 0; i < count; i++)
        {
            float randomValue = UnityEngine.Random.value;

            Vector3 spawnCenter = (randomValue < 0.5f) ? centerFirstCircle.position : centerSecondCircle.position;
            var position = spawnCenter + (Vector3) UnityEngine.Random.insideUnitCircle * spawnRadius;

            var enemy = Instantiate(CurrentWave.enemyPrefab, position, Quaternion.identity);
            SpawnedEnemies.Add(enemy);

            enemy.OnEnemyDeath += Enemy_OnEnemyDeath;
        }
    }

    private void Enemy_OnEnemyDeath(Enemy enemy)
    {
        SpawnedEnemies.Remove(enemy);
        TotalEnemiesAlive--;
        enemy.OnEnemyDeath -= Enemy_OnEnemyDeath;

        if (AreAllEnemiesDeath())
        {
            OnAllEnemiesDead?.Invoke();
        }
    }

    private bool AreAllEnemiesDeath()
    {
        return TotalEnemiesAlive == 0;
    }

    private void DrawSpawnCircleGizmo(Vector3 center, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(center, spawnRadius);
    }

    private void OnDrawGizmosSelected()
    {
        DrawSpawnCircleGizmo(centerFirstCircle.position, Color.green);
        DrawSpawnCircleGizmo(centerSecondCircle.position, Color.red);
    }
}