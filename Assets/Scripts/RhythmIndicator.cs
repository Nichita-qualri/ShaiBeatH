using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class RhythmIndicator : MonoBehaviour
{
    [Header("Components")]
    public RhythmManager rhythmManager;
    public TextMeshProUGUI tapText;
    public RectTransform beatIndicator;
    [Header("Wave")]
    public int wavePoints = 32;
    public float waveWidth = 900f;
    public float waveHeight = 40f;
    [Header("Settings")]
    public float barWidth = 900f;
    [HideInInspector] public bool isInTapWindow = false;
    private float _timer;
    private float _beatInterval;
    private LineRenderer _lineRenderer;
    void Start()
    {
        if (rhythmManager == null)
        {
            rhythmManager = FindObjectOfType<RhythmManager>();
        }
        _beatInterval = 60f / rhythmManager.bpm;
        if (tapText != null) tapText.alpha = 0f;
        // Create the LineRenderer for the wave
        GameObject waveObj = new GameObject("Wave");
        waveObj.transform.SetParent(transform);
        waveObj.transform.localPosition = Vector3.zero;
        waveObj.layer = gameObject.layer;
        _lineRenderer = waveObj.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = wavePoints;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        Material mat = new Material(Shader.Find("Sprites/Default"));
        if (mat != null) _lineRenderer.material = mat;
        _lineRenderer.startColor = new Color(0.91f, 0.75f, 0.44f);
        _lineRenderer.endColor = new Color(0.91f, 0.75f, 0.44f);
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.sortingOrder = 10;
    }
    void Update()
    {
        if (rhythmManager == null) return;
        _timer += Time.deltaTime;
        if (_timer >= _beatInterval)
            _timer = 0f;
        float progress = _timer / _beatInterval;
        isInTapWindow = progress > 0.6f;
        DrawWave(progress);
        if (_lineRenderer != null)
        {
            if (isInTapWindow)
            {
                _lineRenderer.startColor = Color.white;
                _lineRenderer.endColor = Color.white;
            }
            else
            {
                _lineRenderer.startColor = new Color(0.91f, 0.75f, 0.44f);
                _lineRenderer.endColor = new Color(0.91f, 0.75f, 0.44f);
            }
        }
        if (tapText != null)
        {
            if (isInTapWindow)
                tapText.alpha = 1f;
            else
                tapText.alpha = Mathf.Lerp(tapText.alpha, 0f, Time.deltaTime * 8f);
        }
        if (beatIndicator != null)
        {
            float x = Mathf.Lerp(-waveWidth / 2f, waveWidth / 2f, progress);
            float y = Mathf.Sin(progress * Mathf.PI * 2f) * waveHeight;
            beatIndicator.anchoredPosition = new Vector2(x, y);
            float scale = isInTapWindow ? 1.5f : 1f;
            beatIndicator.localScale = Vector3.Lerp(
                beatIndicator.localScale,
                Vector3.one * scale,
                Time.deltaTime * 10f
            );
        }
    }
    void DrawWave(float progress)
    {
        if (_lineRenderer == null) return;
        for (int i = 0; i < wavePoints; i++)
        {
            float t = (float)i / (wavePoints - 1);
            float x = Mathf.Lerp(-waveWidth / 2f, waveWidth / 2f, t);
            float y = Mathf.Sin((t + progress) * Mathf.PI * 2f) * waveHeight;
            _lineRenderer.SetPosition(i, new Vector3(
                transform.position.x + x * 0.01f,
                transform.position.y + y * 0.1f,
                0
            ));
        }
    }
}