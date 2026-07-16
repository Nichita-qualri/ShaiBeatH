using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Music Layers")]
    public AudioClip layerDrums;
    public AudioClip layerTabla;
    public AudioClip layerBass;
    public AudioClip layerMelody;
    public AudioClip layerEpic;
    [Header("Sounds")]
    public AudioClip tapGood;
    public AudioClip tapBad;
    public AudioClip ambient;
    [Header("Settings")]
    public float musicVolume = 0.8f;
    public float sfxVolume = 1f;
    public float ambientVolume = 0.4f;
    public float fadeSpeed = 2f;
    private AudioSource[] _layers;
    private AudioSource _sfxSource;
    private AudioSource _ambientSource;
    private int _currentLayer = 0;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Create 5 AudioSources for the layers
        _layers = new AudioSource[5];
        AudioClip[] clips = { layerDrums, layerTabla, layerBass, layerMelody, layerEpic };
        for (int i = 0; i < 5; i++)
        {
            _layers[i] = gameObject.AddComponent<AudioSource>();
            _layers[i].clip = clips[i];
            _layers[i].loop = true;
            _layers[i].volume = 0f;
            _layers[i].playOnAwake = false;
        }
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.volume = sfxVolume;
        _ambientSource = gameObject.AddComponent<AudioSource>();
        _ambientSource.loop = true;
        _ambientSource.volume = ambientVolume;
    }
    void Start()
    {
        // Start all layers simultaneously but silent
        foreach (var layer in _layers)
            layer.Play();
        _ambientSource.clip = ambient;
        _ambientSource.Play();
        _currentLayer = 0;
    }
    void Update()
    {
        // Smoothly adjust layer volumes
        for (int i = 0; i < _layers.Length; i++)
        {
            float targetVolume = i <= _currentLayer ? musicVolume : 0f;
            _layers[i].volume = Mathf.Lerp(
                _layers[i].volume,
                targetVolume,
                fadeSpeed * Time.deltaTime
            );
        }
    }
    public void OnGoodTap()
    {
        // Add next layer
        if (_currentLayer < _layers.Length - 1)
            _currentLayer++;
        if (tapGood != null)
            _sfxSource.PlayOneShot(tapGood, sfxVolume);
    }
    public void OnBadTap()
    {
        // Remove all layers except the first
        _currentLayer = 0;
        if (tapBad != null)
            _sfxSource.PlayOneShot(tapBad, sfxVolume);
    }
    public void StopAll()
    {
        foreach (var layer in _layers)
            layer.Stop();
        _ambientSource.Stop();
    }
    public void SetSoundEnabled(bool enabled)
    {
        _sfxSource.volume = enabled ? sfxVolume : 0f;
    }
}