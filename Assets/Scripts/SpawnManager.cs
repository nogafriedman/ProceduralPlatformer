using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public static event Action<GameObject, float> OnPlatformSpawned;
    private float screenHalfWidth;

    [Header("Player")]
    [SerializeField] private Transform player;


    [Header("Walls")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private int initialWallCount = 4;
    [SerializeField] private float wallSpawnAhead = 10f;
    [SerializeField] private float wallXPadding = 0.5f;
    private float wallHeight;

    [Header("Platforms")]
    [SerializeField] public GameObject[] platformPrefabs;
    [SerializeField] private int initialPlatformCount = 10;
    [SerializeField] private float platformSpawnAhead = 10f;
    [SerializeField] private float platformSpacing = 3f;
    [SerializeField] private float initialPlatformY = 5f;
    private float platformXMin;
    private float platformXMax;

    [Header("Platform Spawning")]
    [SerializeField] private float[] platformSpawnChances;

    [Header("Platform Types")]
    [SerializeField] private PowerUpSpawner2D[] powerUpSpawners;

    [Header("Platform Number Board")]
    [SerializeField] private GameObject boardPrefab;

    private int nextFloorIndex = 1;

    private readonly List<GameObject> leftWallPool = new List<GameObject>();
    private readonly List<GameObject> rightWallPool = new List<GameObject>();
    private readonly List<GameObject> platformPool = new List<GameObject>();
    private float nextWallY;
    private float nextPlatformY;

    private void Awake()
    {
        if (powerUpSpawners == null || powerUpSpawners.Length == 0)
        {
#if UNITY_2023_1_OR_NEWER
            powerUpSpawners = UnityEngine.Object.FindObjectsByType<PowerUpSpawner2D>(FindObjectsSortMode.None);
#else
            powerUpSpawners = UnityEngine.Object.FindObjectsOfType<PowerUpSpawner2D>();
#endif
        }

        screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        wallHeight = CalculateWallHeight();
        nextPlatformY = initialPlatformY;

        InitWalls();

        var leftCol = leftWallPool[0].GetComponent<Collider2D>();
        var rightCol = rightWallPool[0].GetComponent<Collider2D>();
        platformXMin = leftCol.bounds.max.x;
        platformXMax = rightCol.bounds.min.x;   

        InitPlatforms();
    }

    private void Update()
    {
        while (nextWallY - player.position.y < wallSpawnAhead)
            RespawnWall();

        while (nextPlatformY - player.position.y < platformSpawnAhead)
            RespawnPlatform();
    }

    private void InitWalls()
    {
        for (int i = 0; i < initialWallCount; i++)
        {
            var left = Instantiate(wallPrefab, new Vector3(-screenHalfWidth - wallXPadding, nextWallY, 0f), Quaternion.identity, transform);
            leftWallPool.Add(left);

            var right = Instantiate(wallPrefab, new Vector3(+screenHalfWidth + wallXPadding, nextWallY, 0f), Quaternion.identity, transform);
            rightWallPool.Add(right);

            nextWallY += wallHeight;
        }
    }

    private void InitPlatforms()
    {
        for (int i = 0; i < initialPlatformCount; i++)
        {
            var chosenPrefab = GetRandomPlatformPrefab();

            float halfPlatformWidth = chosenPrefab.GetComponent<SpriteRenderer>().bounds.size.x / 2f;
            platformXMin = halfPlatformWidth;
            platformXMax = -halfPlatformWidth;

            var x = UnityEngine.Random.Range(platformXMin, platformXMax);

            var platform = Instantiate(chosenPrefab, new Vector3(x, nextPlatformY, 0f), Quaternion.identity, transform);

            var indexTag = platform.GetComponent<PlatformIndex>() ?? platform.AddComponent<PlatformIndex>();
            indexTag.floorIndex = nextFloorIndex++;

            // If this is a multiple of 10, spawn a board
            if (indexTag.floorIndex % 10 == 0)
            {
                SpawnBoard(indexTag.floorIndex, platform.transform);
            }

            platformPool.Add(platform);
            nextPlatformY += platformSpacing;

            foreach (var s in powerUpSpawners)
                s.NotifyCreatedFloor(platform, nextPlatformY);
        }
    }

    private void RespawnWall()
    {
        var left = leftWallPool[0];
        leftWallPool.RemoveAt(0);
        left.transform.position = new Vector3(-screenHalfWidth - wallXPadding, nextWallY, 0f);
        leftWallPool.Add(left);

        var right = rightWallPool[0];
        rightWallPool.RemoveAt(0);
        right.transform.position = new Vector3(+screenHalfWidth + wallXPadding, nextWallY, 0f);
        rightWallPool.Add(right);

        nextWallY += wallHeight;
    }

    private void RespawnPlatform()
    {
        var oldPlatform = platformPool[0];
        platformPool.RemoveAt(0);
        Destroy(oldPlatform);

        var chosenPrefab = GetRandomPlatformPrefab();

        var x = UnityEngine.Random.Range(platformXMin, platformXMax);
        var newPlatform = Instantiate(chosenPrefab, new Vector3(x, nextPlatformY, 0f), Quaternion.identity, transform);
        platformPool.Add(newPlatform);

        var indexTag = newPlatform.GetComponent<PlatformIndex>() ?? newPlatform.AddComponent<PlatformIndex>();
        indexTag.floorIndex = nextFloorIndex++;

        // If this is a multiple of 10, spawn a board
        if (indexTag.floorIndex % 10 == 0)
        {
            SpawnBoard(indexTag.floorIndex, newPlatform.transform);
        }

        OnPlatformSpawned?.Invoke(newPlatform, nextPlatformY);

        nextPlatformY += platformSpacing;
    }

    private GameObject GetRandomPlatformPrefab()
    {
        float totalChance = 0f;
        foreach (float chance in platformSpawnChances)
        {
            totalChance += chance;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        for (int i = 0; i < platformPrefabs.Length; i++)
        {
            cumulativeChance += platformSpawnChances[i];
            if (randomValue <= cumulativeChance)
            {
                return platformPrefabs[i];
            }
        }
        return platformPrefabs[0]; // Fallback
    }

    private float CalculateWallHeight()
    {
        if (!wallPrefab) return 0f;

        var sr = wallPrefab.GetComponent<SpriteRenderer>();
        if (sr)
            return sr.bounds.size.y;

        var col = wallPrefab.GetComponent<Collider2D>();
        if (col)
            return col.bounds.size.y;

        return 1f;  // Fallback if no SpriteRenderer
    }

        private void SpawnBoard(int floorNumber, Transform platform)
        {
            if (boardPrefab != null)
            {
                var col = platform.GetComponentInChildren<Collider2D>();
                float platformTop = platform.position.y;
                if (col != null)
                    platformTop = col.bounds.max.y;

                Vector3 pos = new Vector3(platform.position.x, platformTop, 0f);

                var board = Instantiate(boardPrefab, pos, Quaternion.identity, platform);

                Vector3 parentScale = platform.localScale;
                board.transform.localScale = new Vector3(
                    0.7f / parentScale.x,
                    0.7f / parentScale.y,
                    1f / parentScale.z
                );

                board.transform.localRotation = Quaternion.identity;
                board.transform.localPosition += new Vector3(0.8f, 0f, 0f);

                var text = board.GetComponentInChildren<TMPro.TextMeshPro>();
                if (text != null)
                    text.text = floorNumber.ToString();
            }
        }
}
