using UnityEngine;
public class SpiceMarker : MonoBehaviour
{
    [Header("Settings")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.15f;
    [Header("Biome Sprites")]
    public Sprite spriteDay;
    public Sprite spriteNight;
    public Sprite spriteUnderground;
    public System.Action onCollected;
    private Vector3 _startScale;
    private HarvesterController _harvester;
    private bool _collected = false;
    private int _savedMultiplier = 1;
    void Start()
    {
        _startScale = transform.localScale;
        _harvester = FindObjectOfType<HarvesterController>();
        ApplyBiomeSprite();
    }
    void ApplyBiomeSprite()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        if (level <= 5)
            sr.sprite = spriteDay;
        else if (level <= 10)
            sr.sprite = spriteNight;
        else
            sr.sprite = spriteUnderground;
    }
    void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = _startScale * pulse;
    }
    void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        if (_collected) return;
        RhythmIndicator rhythmIndicator = FindObjectOfType<RhythmIndicator>();
        RhythmManager rhythm = FindObjectOfType<RhythmManager>();
        if (rhythmIndicator.isInTapWindow)
        {
            _collected = true;
            rhythm.RegisterGoodTap();
            _savedMultiplier = rhythm.GetComboMultiplier();
            _harvester.TapOnMarker(transform.position, this);
        }
        else
        {
            rhythm.RegisterBadTap();
        }
    }
    public void Collect()
    {
        GameManager.Instance?.AddSpice(_savedMultiplier);
        onCollected?.Invoke();
        Destroy(gameObject);
    }
}