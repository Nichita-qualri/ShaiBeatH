using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI musicToggleText;
    public TextMeshProUGUI soundToggleText;
    public TextMeshProUGUI vibrationToggleText;

    private bool _musicOn;
    private bool _soundOn;
    private bool _vibrationOn;

    void Start()
    {
        _musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        _soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        _vibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        UpdateUI();
    }

    void UpdateUI()
    {
        musicToggleText.text = _musicOn ? "ON" : "OFF";
        soundToggleText.text = _soundOn ? "ON" : "OFF";
        vibrationToggleText.text = _vibrationOn ? "ON" : "OFF";
    }

    public void ToggleMusic()
    {
        _musicOn = !_musicOn;
        PlayerPrefs.SetInt("MusicOn", _musicOn ? 1 : 0);
        PlayerPrefs.Save();

        if (MenuMusicManager.Instance != null)
        {
            if (_musicOn)
                MenuMusicManager.Instance.PlayMusic();
            else
                MenuMusicManager.Instance.StopMusic();
        }

        UpdateUI();
    }

    public void ToggleSound()
    {
        _soundOn = !_soundOn;
        PlayerPrefs.SetInt("SoundOn", _soundOn ? 1 : 0);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSoundEnabled(_soundOn);

        UpdateUI();
    }

    public void ToggleVibration()
    {
        _vibrationOn = !_vibrationOn;
        PlayerPrefs.SetInt("VibrationOn", _vibrationOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}