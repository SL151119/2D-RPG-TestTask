using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }

    [Header("Levels To Load")]
    [SerializeField] private SceneField shopLevel;
    [SerializeField] private SceneField[] levelsToLoad;

    public int LevelIndex { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LevelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
    }

    public void IncreaseLevelIndex() => LevelIndex++;

    public void ResetLevelIndex() => LevelIndex = 0;

    public void LoadLevel(bool loadNextLevel)
    {
        var action = loadNextLevel ? (Action)LoadNextLevel : LoadShopLevel;
        action();
    }

    private void LoadShopLevel() => StartCoroutine(LoadSceneAsyncCoroutine(shopLevel));

    private void LoadNextLevel()
    {
        if (LevelIndex < levelsToLoad.Length)
        {
            SceneField nextLevel = levelsToLoad[LevelIndex];
            StartCoroutine(LoadSceneAsyncCoroutine(nextLevel));

            PlayerPrefs.SetInt(Constants.LEVEL_INDEX, LevelIndex);
        }
    }

    private IEnumerator LoadSceneAsyncCoroutine(SceneField scene)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public bool IsLastLevel()
    {
        SceneField currentScene = levelsToLoad.ElementAt(LevelIndex);
        SceneField lastLevel = levelsToLoad.Last();

        return currentScene != null && currentScene.Equals(lastLevel);
    }
}
