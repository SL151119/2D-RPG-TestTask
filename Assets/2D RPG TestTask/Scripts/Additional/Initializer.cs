using UnityEngine;

public static class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Execute()
    {
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("PersistentObjects")));
    }
}