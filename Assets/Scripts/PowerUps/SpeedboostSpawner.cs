using UnityEngine;

public class SpeedBoostSpawner : PowerUpSpawner
{
    [SerializeField] private GameObject speedBoostPrefab;
    [SerializeField, Range(0f, 1f)] private float spawnChance = 0.15f;
    [SerializeField] private float verticalOffset = 1f;

    public override void NotifyCreatedFloor(GameObject platform, float nextY)
    {
        if (!speedBoostPrefab) return;

        if (Random.value < spawnChance)
        {
            Vector3 spawnPos = platform.transform.position + Vector3.up * verticalOffset;
            Instantiate(speedBoostPrefab, spawnPos, Quaternion.identity, platform.transform);
        }
    }
}
