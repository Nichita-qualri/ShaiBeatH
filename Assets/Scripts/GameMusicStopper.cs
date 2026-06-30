using UnityEngine;

public class GameMusicStopper : MonoBehaviour
{
    void Start()
    {
        if (MenuMusicManager.Instance != null)
            MenuMusicManager.Instance.StopMusic();
    }
}