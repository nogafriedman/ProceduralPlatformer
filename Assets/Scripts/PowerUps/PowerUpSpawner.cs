using UnityEngine;

/// <summary>
/// Base class for all power-up spawners.
/// Listens to SpawnManager events and decides whether to place
/// a power-up on a newly created platform.
/// </summary>
public abstract class PowerUpSpawner : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        SpawnManager.OnPlatformSpawned += HandlePlatformSpawned;
    }

    protected virtual void OnDisable()
    {
        SpawnManager.OnPlatformSpawned -= HandlePlatformSpawned;
    }

    private void HandlePlatformSpawned(GameObject platform, float nextY)
    {
        NotifyCreatedFloor(platform, nextY);
    }

    // Called by SpawnManager when a new platform is created.
    public abstract void NotifyCreatedFloor(GameObject platform, float nextY);
}
