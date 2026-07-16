using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WormController : MonoBehaviour
{
    [Header("Settings")]
    public float normalSpeed = 0f;
    public float angrySpeed = 0.3f;
    public float dangerDistance = 2f;
    public float deathDistance = 0.8f;
    [Header("UI")]
    public Image dangerOverlay;
    public TextMeshProUGUI dodgeText;
    private Transform _harvester;
    private float _currentSpeed = 0f;
    private RhythmManager _rhythmManager;
    private bool _inDanger = false;
    private bool _isDead = false;
    private Animator _animator;
    void Start()
    {
        _harvester = FindObjectOfType<HarvesterController>().transform;
        _rhythmManager = FindObjectOfType<RhythmManager>();
        _rhythmManager.onGoodTap.AddListener(OnGoodTap);
        _rhythmManager.onBadTap.AddListener(OnBadTap);
        _animator = GetComponent<Animator>();
        if (dodgeText) dodgeText.alpha = 0f;
        if (dangerOverlay) dangerOverlay.color = new Color(1, 0, 0, 0);
    }
    void Update()
    {
        if (_harvester == null || _isDead) return;
        transform.position = Vector3.MoveTowards(
            transform.position,
            _harvester.position,
            _currentSpeed * Time.deltaTime
        );
        float dist = Vector3.Distance(transform.position, _harvester.position);
        _inDanger = dist <= dangerDistance;
        if (dist <= deathDistance)
        {
            PlayerDied();
            return;
        }
        if (dangerOverlay != null)
        {
            float alpha = _inDanger ? 0.3f : 0f;
            Color c = dangerOverlay.color;
            dangerOverlay.color = new Color(c.r, c.g, c.b,
                Mathf.Lerp(c.a, alpha, Time.deltaTime * 5f));
        }
        if (dodgeText != null)
        {
            dodgeText.alpha = _inDanger ?
                Mathf.Lerp(dodgeText.alpha, 1f, Time.deltaTime * 5f) :
                Mathf.Lerp(dodgeText.alpha, 0f, Time.deltaTime * 5f);
        }
    }
    void PlayerDied()
    {
        HarvesterController harvester = FindObjectOfType<HarvesterController>();
        if (harvester != null && harvester.TryUseArmor())
        {
            ResetWorm(harvester.transform.position + Vector3.down * 4f);
            return;
        }
        _isDead = true;
        _currentSpeed = 0f;
        // Stop the game
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
        // Hide the harvester
        if (harvester != null)
            harvester.gameObject.SetActive(false);
        // Clear the danger UI
        if (dodgeText != null) dodgeText.alpha = 0f;
        if (dangerOverlay != null)
            dangerOverlay.color = new Color(1, 0, 0, 0f);
        // Play the animation
        if (_animator != null)
            _animator.SetTrigger("Attack");
        // Wait for the animation to finish, then show the DeathScreen
        float animLength = 2f;
        Invoke(nameof(ShowDeath), animLength);
    }
    void ShowDeath()
    {
        int score = GameManager.Instance != null ? GameManager.Instance.GetSpice() : 0;
        DeathScreen.Instance?.ShowDeathScreen(score);
    }
    void OnGoodTap()
    {
        _currentSpeed = normalSpeed;
    }
    void OnBadTap()
    {
        _currentSpeed = angrySpeed;
    }
    public void ResetWorm(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        _currentSpeed = 0f;
    }
    public void StopWorm()
    {
        _isDead = true;
        _currentSpeed = 0f;
    }
}