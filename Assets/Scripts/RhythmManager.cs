using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    [Header("Rhythm")]
    public float bpm = 40f;
    public float tapWindowMs = 200f;

    [Header("Events")]
    public UnityEvent onBeat;
    public UnityEvent onGoodTap;
    public UnityEvent onBadTap;

    [HideInInspector] public int combo = 0;

    private float _beatInterval;
    private float _timer;
    private bool _waitingForTap;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _beatInterval = 60f / bpm;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _beatInterval)
        {
            _timer = 0f;
            _waitingForTap = true;
            onBeat?.Invoke();
        }
    }

    public void RegisterGoodTap()
    {
        combo++;
        onGoodTap?.Invoke();
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnGoodTap();
    }

    public void RegisterBadTap()
    {
        combo = 0;
        onBadTap?.Invoke();
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnBadTap();
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.3f, 0.1f);
    }

    public bool IsInTapWindow()
    {
        return _waitingForTap;
    }

    public int GetComboMultiplier()
    {
        if (combo >= 20) return 4;
        if (combo >= 10) return 3;
        if (combo >= 5) return 2;
        return 1;
    }
}