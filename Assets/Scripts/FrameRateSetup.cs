using UnityEngine;

public static class FrameRateSetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Application.targetFrameRate = 60;
    }
}