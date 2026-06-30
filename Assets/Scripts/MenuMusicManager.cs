using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance;

    [Header("Музыка")]
    public AudioClip menuMusic;

    private AudioSource _musicSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.clip = menuMusic;
        _musicSource.loop = true;
        _musicSource.volume = 0.5f;

        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        if (musicOn)
            _musicSource.Play();
    }

    public void StopMusic()
    {
        if (_musicSource != null)
            _musicSource.Stop();
    }

    public void PlayMusic()
    {
        if (_musicSource != null && !_musicSource.isPlaying)
            _musicSource.Play();
    }
}