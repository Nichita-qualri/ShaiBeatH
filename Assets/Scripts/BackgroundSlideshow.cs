using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundSlideshow : MonoBehaviour
{
    [Header("Фоны")]
    public Sprite[] backgrounds;
    public Image backgroundImage1;
    public Image backgroundImage2;

    [Header("Настройки")]
    public float displayTime = 4f;
    public float fadeTime = 1.5f;

    private int _currentIndex = 0;
    private bool _isImage1Active = true;

    void Start()
    {
        if (backgrounds.Length == 0) return;
        backgroundImage1.sprite = backgrounds[0];
        backgroundImage2.sprite = backgrounds[1 % backgrounds.Length];
        backgroundImage1.color = new Color(1, 1, 1, 1);
        backgroundImage2.color = new Color(1, 1, 1, 0);
        StartCoroutine(SlideShow());
    }

    IEnumerator SlideShow()
    {
        while (true)
        {
            yield return new WaitForSeconds(displayTime);

            _currentIndex = (_currentIndex + 1) % backgrounds.Length;
            int nextIndex = (_currentIndex + 1) % backgrounds.Length;

            if (_isImage1Active)
            {
                backgroundImage2.sprite = backgrounds[_currentIndex];
                yield return StartCoroutine(Fade(backgroundImage1, backgroundImage2));
            }
            else
            {
                backgroundImage1.sprite = backgrounds[_currentIndex];
                yield return StartCoroutine(Fade(backgroundImage2, backgroundImage1));
            }

            _isImage1Active = !_isImage1Active;
        }
    }

    IEnumerator Fade(Image fadeOut, Image fadeIn)
    {
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            fadeOut.color = new Color(1, 1, 1, 1 - t);
            fadeIn.color = new Color(1, 1, 1, t);
            yield return null;
        }
        fadeOut.color = new Color(1, 1, 1, 0);
        fadeIn.color = new Color(1, 1, 1, 1);
    }
}