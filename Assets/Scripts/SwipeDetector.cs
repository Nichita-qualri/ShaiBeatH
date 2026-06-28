using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    [Header("Настройки")]
    public float minSwipeDistance = 50f;
    public HarvesterController harvester;
    public WormController worm;

    private Vector2 _startTouch;
    private bool _swiping = false;
    private int _swipesUsed = 0;
    private int _maxSwipes = 1;

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

        Debug.Log("Свайп: " + direction + " использовано: " + _swipesUsed + "/" + _maxSwipes);

        Vector3 newPos = harvester.transform.position + direction * 3f;
        newPos.x = Mathf.Clamp(newPos.x, -4f, 4f);
        newPos.y = Mathf.Clamp(newPos.y, -2f, 3.5f);
        harvester.DodgeTo(newPos);

        Vector3 wormResetPos = harvester.transform.position + (-direction) * 4f;
        wormResetPos.x = Mathf.Clamp(wormResetPos.x, -4f, 4f);
        wormResetPos.y = Mathf.Clamp(wormResetPos.y, -2f, 3.5f);
        worm.ResetWorm(wormResetPos);
    }
}