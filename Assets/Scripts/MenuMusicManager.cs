using UnityEngine;
using UnityEngine.UI;

public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance;

    [Header("Music")]
    public AudioClip menuMusic;

    [Header("Sound")]
    public AudioClip buttonClick;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

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

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.volume = 1f;

        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        if (musicOn)
            _musicSource.Play();

        AddSoundToAllButtons();
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        AddSoundToAllButtons();
    }

    void AddSoundToAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            Button b = btn;
            b.onClick.AddListener(() => PlayClick());
        }
    }

    public void PlayClick()
    {
        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        if (soundOn && buttonClick != null)
            _sfxSource.PlayOneShot(buttonClick);
    }

    public void StopMusic()
    {
        if (_musicSource != null)
            _musicSource.Stop();
    }

    public void PlayMusic()
    {
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        if (_musicSource != null && !_musicSource.isPlaying && musicOn)
            _musicSource.Play();
    }
}