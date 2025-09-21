using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class PowerUpPickup : MonoBehaviour
{
    private bool _consumed;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            Debug.Log("Trigger entered by: " + other.name);


        if (_consumed) return;
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerController>(out var player))
        {
                    Debug.Log("Applying power-up to: " + player.name);
            ApplyPowerUp(player);
        }

        _consumed = true;
        Destroy(gameObject);
    }

    protected abstract void ApplyPowerUp(PlayerController player);
}