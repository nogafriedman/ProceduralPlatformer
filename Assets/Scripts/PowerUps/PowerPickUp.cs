using UnityEngine;

/// <summary>
/// Base class for all power-up collectibles.
/// Handles trigger detection, pickup logic,
/// and sound/VFX feedback.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class PowerUpPickup : MonoBehaviour
{

    [Header("Pickup Feedback")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupVFX;
    [SerializeField] private float vfxLifetime = 1.5f;

    private bool _consumed;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed) return;
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerController>(out var player))
        {
            ApplyPowerUp(player);
            PlayFeedback();
        }

        _consumed = true;
        Destroy(gameObject);
    }

    /// <summary>Called when the player collides with the power-up.</summary>
    protected abstract void ApplyPowerUp(PlayerController player);

    // <summary>Plays optional sound & VFX when picked up.</summary>
    private void PlayFeedback()
    {
        if (pickupSound)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        if (pickupVFX)
        {
            var vfx = Instantiate(pickupVFX, transform.position, Quaternion.identity);
            Destroy(vfx, vfxLifetime);
        }
    }
}