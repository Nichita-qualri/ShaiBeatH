using UnityEngine;
public class SpiceSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject spiceMarkerPrefab;
    [Header("Spawn Zone")]
    public float minX = -4f;
    public float maxX = 4f;
    public float minY = -1f;
    public float maxY = 5f;
    [Header("Settings")]
    public int maxMarkersOnScreen = 3;
    public float minDistanceFromWorm = 3f;
    private int _currentMarkers = 0;
    private Transform _worm;
    void Start()
    {
        _worm = FindObjectOfType<WormController>().transform;
        SpawnMarker();
        SpawnMarker();
    }
    public void SpawnMarker()
    {
        if (_currentMarkers >= maxMarkersOnScreen) return;
        Vector3 spawnPos = GetSafePosition();
        GameObject marker = Instantiate(spiceMarkerPrefab, spawnPos, Quaternion.identity);
        marker.GetComponent<SpiceMarker>().onCollected = OnMarkerCollected;
        _currentMarkers++;
    }
    Vector3 GetSafePosition()
    {
        Vector3 pos;
        int attempts = 0;
        do
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            pos = new Vector3(x, y, 0);
            attempts++;
        }
        while (_worm != null &&
               Vector3.Distance(pos, _worm.position) < minDistanceFromWorm &&
               attempts < 20);
        return pos;
    }
    void OnMarkerCollected()
    {
        _currentMarkers--;
        SpawnMarker();
    }
    public void StopSpawning()
    {
        maxMarkersOnScreen = 0;
    }
}