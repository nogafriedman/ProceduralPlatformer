using UnityEngine;

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
