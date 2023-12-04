using System;
using System.Collections;
using UnityEngine;

public static class CoroutineExtensions
{
    public static void UniversalWait(this MonoBehaviour monoBehaviour, float time, Action action)
    {
        monoBehaviour.StartCoroutine(UniversalWaitCoroutine(time, action));
    }

    public static void UniversalSequence(this MonoBehaviour monoBehaviour, float time, Action firstAction, Action secondAction, Action thirdAction)
    {
        monoBehaviour.StartCoroutine(UniversalSequenceCoroutine(time, firstAction, secondAction, thirdAction));
    }

    public static void UniversalSequenceTwo(this MonoBehaviour monoBehaviour, float time, Action firstAction, Action secondAction)
    {
        monoBehaviour.StartCoroutine(UniversalSequenceTwoCoroutine(time, firstAction, secondAction));
    }

    private static IEnumerator UniversalWaitCoroutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    private static IEnumerator UniversalSequenceCoroutine(float time, Action firstAction, Action secondAction, Action thirdAction)
    {
        firstAction();
        yield return new WaitForSeconds(time);
        secondAction();
        thirdAction();
    }

    private static IEnumerator UniversalSequenceTwoCoroutine(float time, Action firstAction, Action secondAction)
    {
        firstAction();
        yield return new WaitForSeconds(time);
        secondAction();
    }
}
