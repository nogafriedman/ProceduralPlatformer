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
    [SerializeField] private float wallXPadding = 0.3f;
    private float wallHeight;

    [Header("Platforms")]
    [SerializeField] private GameObject[] platformPrefabs;
    [SerializeField] private int initialPlatformCount = 10;
    [SerializeField] private float platformSpawnAhead = 10f;
    [SerializeField] private float platformSpacing = 3f;
    [SerializeField] private float initialPlatformY = 5f;
    private float platformXMin;
    private float platformXMax;

    [Header("Platform Spawning")]
    [SerializeField] private float[] platformSpawnChances;

    [Header("Platform Number Board")]
    [SerializeField] private GameObject boardPrefab;

    private int nextFloorIndex = 1;

    [Header("Pools")]

    private readonly List<GameObject> leftWallPool = new List<GameObject>();
    private readonly List<GameObject> rightWallPool = new List<GameObject>();
    private readonly Dictionary<GameObject, Queue<GameObject>> platformPools = new();
    private readonly List<GameObject> platformPool = new();
    private readonly Queue<GameObject> boardPool = new Queue<GameObject>();
    private float nextWallY;
    private float nextPlatformY;

    private void Awake()
    {
        screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        wallHeight = CalculateWallHeight();
        nextPlatformY = initialPlatformY;

        InitWalls();

        var leftCol = leftWallPool[0].GetComponent<Collider2D>();
        var rightCol = rightWallPool[0].GetComponent<Collider2D>();
        platformXMin = leftCol.bounds.max.x;
        platformXMax = rightCol.bounds.min.x;

        foreach (var prefab in platformPrefabs)
        {
            platformPools[prefab] = new Queue<GameObject>();
        }

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

        foreach (Transform child in oldPlatform.transform)
        {
        if (child.CompareTag("Board"))
        {
            RecycleBoard(child.gameObject);
        }
        }
        RecyclePlatform(oldPlatform);

        var chosenPrefab = GetRandomPlatformPrefab();
        var x = UnityEngine.Random.Range(platformXMin, platformXMax);

        var newPlatform = GetPooledPlatform(chosenPrefab);

        newPlatform.transform.position = new Vector3(x, nextPlatformY, 0f);
        newPlatform.transform.SetParent(transform, false);
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


    // Helpers

    private GameObject GetPooledPlatform(GameObject prefab)
    {
        if (platformPools.TryGetValue(prefab, out var pool) && pool.Count > 0)
        {
            var platform = pool.Dequeue();
            platform.SetActive(true);
            return platform;
        }

        // If none available, instantiate new
        return Instantiate(prefab, transform);
    }

    private void RecyclePlatform(GameObject platform)
    {
        platform.SetActive(false);

        foreach (var prefab in platformPrefabs)
        {
            if (platform.name.StartsWith(prefab.name))
            {
                platformPools[prefab].Enqueue(platform);
                return;
            }
        }
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
        if (boardPrefab == null) return;

        GameObject board;

        if (boardPool.Count > 0)
        {
            board = boardPool.Dequeue();
            board.SetActive(true);
        }
        else
        {
            board = Instantiate(boardPrefab);
        }

        board.transform.SetParent(platform, false);

        var col = platform.GetComponentInChildren<Collider2D>();
        float platformTop = (col != null) ? col.bounds.max.y : platform.position.y;

        board.transform.position = new Vector3(platform.position.x, platformTop, 0f);
        board.transform.localRotation = Quaternion.identity;
        board.transform.localPosition += new Vector3(0.8f, 0f, 0f);

        // Reset scale relative to parent
        Vector3 parentScale = platform.localScale;
        board.transform.localScale = new Vector3(
            0.7f / parentScale.x,
            0.7f / parentScale.y,
            1f / parentScale.z
        );

        var text = board.GetComponentInChildren<TMPro.TextMeshPro>();
        if (text != null)
            text.text = floorNumber.ToString();

        // var col = platform.GetComponentInChildren<Collider2D>();
        //         float platformTop = platform.position.y;
        //         if (col != null)
        //             platformTop = col.bounds.max.y;

        //         Vector3 pos = new Vector3(platform.position.x, platformTop, 0f);

        //         var board = Instantiate(boardPrefab, pos, Quaternion.identity, platform);

        //         Vector3 parentScale = platform.localScale;
        //         board.transform.localScale = new Vector3(
        //             0.7f / parentScale.x,
        //             0.7f / parentScale.y,
        //             1f / parentScale.z
        //         );

        //         board.transform.localRotation = Quaternion.identity;
        //         board.transform.localPosition += new Vector3(0.8f, 0f, 0f);

        //         var text = board.GetComponentInChildren<TMPro.TextMeshPro>();
        //         if (text != null)
        //             text.text = floorNumber.ToString();
        //     }
    }
    private void RecycleBoard(GameObject board)
    {
        board.SetActive(false);
        board.transform.SetParent(transform, false); // detach
        boardPool.Enqueue(board);
    }

}
