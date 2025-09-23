using UnityEngine;

/// <summary>
/// Spawns a jetpack power-up above newly created platforms.
/// </summary>
public class JetpackSpawner : PowerUpSpawner
{
    [SerializeField] private GameObject jetpackPrefab;
    [SerializeField, Range(0f, 1f)] private float spawnChance = 0.1f;
    [SerializeField] private float verticalOffset = 1.5f;

    public override void NotifyCreatedFloor(GameObject platform, float nextY)
    {
        if (!jetpackPrefab) return;

        if (Random.value < spawnChance)
        {
            Vector3 spawnPos = platform.transform.position + Vector3.up * verticalOffset;
            Instantiate(jetpackPrefab, spawnPos, Quaternion.identity, platform.transform);
        }
    }
}
