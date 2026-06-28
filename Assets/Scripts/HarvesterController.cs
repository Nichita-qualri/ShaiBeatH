using UnityEngine;

public class HarvesterController : MonoBehaviour
{
    [Header("Настройки")]
    public float baseMoveSpeed = 3f;
    public float baseCollectRadius = 0.5f;
    public float rotationSpeed = 10f;

    [Header("Щит")]
    public GameObject shieldEffect;

    private float _moveSpeed;
    private float _collectRadius;
    private bool _hasArmor = false;
    private bool _armorUsed = false;
    private Vector3 _targetPosition;
    private bool _isMoving = false;
    private RhythmManager _rhythmManager;
    private SpiceMarker _targetMarker;
    private ParticleSystem[] _sandDusts;

    void Start()
    {
        _targetPosition = transform.position;
        _rhythmManager = FindObjectOfType<RhythmManager>();
        _sandDusts = GetComponentsInChildren<ParticleSystem>();
        foreach (var dust in _sandDusts) dust.Stop();
        if (shieldEffect != null)
            shieldEffect.SetActive(false);
        ApplyUpgrades();
    }

    void ApplyUpgrades()
    {
        int speedLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetSpeedLevel() : 0;
        int radiusLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetRadiusLevel() : 0;
        int armorLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetArmorLevel() : 0;

        _moveSpeed = baseMoveSpeed + speedLevel * 0.5f;
        _collectRadius = baseCollectRadius + radiusLevel * 0.2f;
        _hasArmor = armorLevel > 0;

        if (shieldEffect != null)
            shieldEffect.SetActive(false);

        int skinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color[] skinColors = {
                new Color(1f, 1f, 1f),
                new Color(0.10f, 0.23f, 0.42f),
                new Color(0.42f, 0.10f, 0.10f),
                new Color(0.23f, 0.16f, 0.06f)
            };
            if (skinIndex < skinColors.Length)
                sr.color = skinColors[skinIndex];
        }
    }

    void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                _moveSpeed * Time.deltaTime
            );

            Vector3 direction = _targetPosition - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            foreach (var dust in _sandDusts)
                if (!dust.isPlaying) dust.Play();

            if (Vector3.Distance(transform.position, _targetPosition) < 0.05f)
            {
                transform.position = _targetPosition;
                _isMoving = false;

                if (_targetMarker != null)
                {
                    _targetMarker.Collect();
                    _targetMarker = null;
                }
            }
        }
        else
        {
            foreach (var dust in _sandDusts)
                if (dust.isPlaying) dust.Stop();
        }
    }

    public void TapOnMarker(Vector3 markerPosition, SpiceMarker marker)
    {
        float clampedX = Mathf.Clamp(markerPosition.x, -4f, 4f);
        float clampedY = Mathf.Clamp(markerPosition.y, -4f, 3.5f);
        _targetPosition = new Vector3(clampedX, clampedY, transform.position.z);
        _targetMarker = marker;
        _isMoving = true;
    }

    public void DodgeTo(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -4f, 4f);
        position.y = Mathf.Clamp(position.y, -4f, 3.5f);
        _targetPosition = position;
        _isMoving = true;
    }

    public bool TryUseArmor()
    {
        if (_hasArmor && !_armorUsed)
        {
            _armorUsed = true;
            Debug.Log("Броня использована!");

            WormController wormController = FindObjectOfType<WormController>();
            if (wormController != null)
            {
                Vector3 resetPos = transform.position + Vector3.down * 4f;
                resetPos.x = Mathf.Clamp(resetPos.x, -4f, 4f);
                resetPos.y = Mathf.Clamp(resetPos.y, -3f, 3.5f);
                wormController.ResetWorm(resetPos);
            }

            if (shieldEffect != null)
            {
                shieldEffect.SetActive(true);
                Invoke(nameof(HideShield), 2f);
            }
            return true;
        }
        return false;
    }

    void HideShield()
    {
        if (shieldEffect != null)
            shieldEffect.SetActive(false);
    }

    public float GetCollectRadius() => _collectRadius;
}
