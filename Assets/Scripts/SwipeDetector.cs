using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    [Header("Settings")]
    public float minSwipeDistance = 50f;
    public HarvesterController harvester;
    public WormController worm;

    private Vector2 _startTouch;
    private bool _swiping = false;
    private int _swipesUsed = 0;
    private int _maxSwipes = 1;

    private float _minY = -1f;
    private float _maxY = 3.5f;
    private float _minX = -4f;
    private float _maxX = 4f;

    void Start()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        _maxSwipes = level >= 8 ? 2 : 1;
        _swipesUsed = 0;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        if (PauseManager.Instance != null && Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(0))
        {
            _startTouch = Input.mousePosition;
            _swiping = true;
        }

        if (Input.GetMouseButtonUp(0) && _swiping)
        {
            Vector2 delta = (Vector2)Input.mousePosition - _startTouch;
            if (Mathf.Abs(delta.x) > minSwipeDistance)
            {
                if (delta.x > 0)
                    DoSwipe(Vector3.right);
                else
                    DoSwipe(Vector3.left);
            }
            _swiping = false;
        }
    }

    void DoSwipe(Vector3 direction)
    {
        if (_swipesUsed >= _maxSwipes) return;
        _swipesUsed++;

        // Двигаем харвестер
        Vector3 newPos = harvester.transform.position + direction * 3f;
        newPos.x = Mathf.Clamp(newPos.x, _minX, _maxX);
        newPos.y = Mathf.Clamp(newPos.y, _minY, _maxY);
        harvester.DodgeTo(newPos);

        // Червь в противоположную сторону
        Vector3 wormResetPos = GetSafeWormPosition();
        worm.ResetWorm(wormResetPos);
    }

    Vector3 GetSafeWormPosition()
    {
        Vector3 harvesterPos = harvester.transform.position;
        float midY = (_minY + _maxY) / 2f;

        float x = Random.Range(_minX, _maxX);
        float y;

        if (harvesterPos.y < midY - 1f)
        {
            // Харвестер внизу — червь вверх
            y = Random.Range(midY + 0.5f, _maxY);
        }
        else if (harvesterPos.y > midY + 1f)
        {
            // Харвестер вверху — червь вниз
            y = Random.Range(_minY, midY - 0.5f);
        }
        else
        {
            // Харвестер посередине — рандомно
            if (Random.value > 0.5f)
                y = Random.Range(midY + 0.5f, _maxY);
            else
                y = Random.Range(_minY, midY - 0.5f);
        }

        return new Vector3(x, y, 0f);
    }
}