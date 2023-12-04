using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Prefab")]
    [SerializeField] private Player playerPrefab;

    [Header("Enemy")]
    [SerializeField] private Enemy enemy;

    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Button_UI respawnButton;

    [Header("Item Spawner Settings")]
    [SerializeField] private List<ItemWorldSpawner> itemSpawnerPrefabs = new List<ItemWorldSpawner>();
    [SerializeField] private float spawnRadius = 3f;

    private readonly float waitForSeconds = 1.5f;

    private Vector3 playerInitialPosition;
    private Vector3 randomSpawnPosition;

    private Player currentPlayerInstance;
    private bool isGameOverUIEnabled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SoundFXManager.Init();
    }

    private void Start()
    {
        isGameOverUIEnabled = false;

        playerInitialPosition = playerPrefab.transform.position;

        SpawnPlayer();

        RespawnItems();

        respawnButton.ClickFunc = () =>
        {
            SoundFXManager.PlaySound(SoundFXManager.Sound.Respawn);
            RespawnPlayer();
        };
    }

    private void Update()
    {
        if (currentPlayerInstance != null && currentPlayerInstance.IsDead)
        {
            if (!isGameOverUIEnabled)
            {
                this.UniversalWait(waitForSeconds, () => gameOverUI.SetActive(true));
                isGameOverUIEnabled = true;
            }
        }
        else
        {
            isGameOverUIEnabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    private void RespawnPlayer()
    {
        DestroyPlayer();    // Destroy the current player instance

        gameOverUI.SetActive(false);

        SceneLoadManager.Instance.LoadLevel(false);

        SpawnPlayer();

        this.UniversalWait(0.2f, () =>
        {
            RespawnItems();
        });
    }

    private void SpawnPlayer()
    {
        currentPlayerInstance = Instantiate(playerPrefab, playerInitialPosition, Quaternion.identity);
        DontDestroyOnLoad(currentPlayerInstance);
        currentPlayerInstance.InitializePlayer();
    }

    private void RespawnItems()
    {
        foreach (var spawnerPrefab in itemSpawnerPrefabs)
        {
            Instantiate(spawnerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        }
    }

    public Vector3 GetRandomSpawnPosition() =>
        randomSpawnPosition = (Vector2)currentPlayerInstance.GetPosition() + Random.insideUnitCircle * spawnRadius;

    private void DestroyPlayer()
    {
        Destroy(currentPlayerInstance.gameObject);
    }

    private void ExitGame()
    {
        PlayerPrefs.DeleteAll();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}