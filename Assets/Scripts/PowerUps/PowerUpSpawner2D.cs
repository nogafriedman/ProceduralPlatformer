using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public sealed class PowerUpSpawner2D : MonoBehaviour
{
    [SerializeField] private GameObject[] powerUpPrefabs;   // ← multiple prefabs
    [SerializeField] private int spawnEveryPlatforms;
    // [SerializeField] private float minLeadAbovePlayer = 2.5f; // world units above player
    private int nextSpawnFloor = 20;

    // set  platform layers in the Inspector
    [SerializeField] private LayerMask platformLayers;

    // placement tuning
    [SerializeField] private float verticalOffset = 0.25f;
    [SerializeField] private float horizontalPadding = 0.3f;
    [SerializeField] private Vector2 overlapCheckSize = new Vector2(0.35f, 0.35f);
    // [SerializeField] private int maxAttempts = 15;

    private GameObject _powerupInstance;

    private void Awake()
    {
        nextSpawnFloor = spawnEveryPlatforms;
    }

    private void Start()
    {
        nextSpawnFloor = spawnEveryPlatforms;
    }

    private bool TrySpawn(GameObject platform, float nextPlatformY)
    {
        if (_powerupInstance != null)
        {
            Destroy(_powerupInstance);
        }
        Collider2D plat = platform.GetComponent<Collider2D>();
        Bounds b = plat.bounds;

        float left = b.min.x + horizontalPadding;
        float right = b.max.x - horizontalPadding;
        if (right <= left)
        {
            return false;
        }

        float x = Random.Range(left, right);
        float y = b.max.y + verticalOffset; // makes sure powerups above platform
        Vector2 pos = new Vector2(x, y);

        // avoid overlapping platforms at the spawn point
        if (Physics2D.OverlapBox(pos, overlapCheckSize, 0f, platformLayers) != null)
        {
            return false;
        }

        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        _powerupInstance = Instantiate(prefab, pos, Quaternion.identity);
        _powerupInstance.transform.SetParent(platform.transform, true);// make platform parent(becuse it needs to move together with platform)

        return true;
    }

    public void NotifyCreatedFloor(GameObject platform, float nextPlatformY)
    {
        if (nextSpawnFloor == 0)
        {
            if (TrySpawn(platform, nextPlatformY))
            {
                nextSpawnFloor = spawnEveryPlatforms;
            }
        }
        else
        {
            nextSpawnFloor--;
        }
    }

}
