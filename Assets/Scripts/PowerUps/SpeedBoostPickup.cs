using UnityEngine;

/// <summary>
/// Pickup component for the speed boost power-up.
/// Creates a SpeedBoostAbility and applies it to the player
/// with the configured multiplier and duration.
/// </summary>
public class SpeedBoostPickup : PowerUpPickup
{
    [SerializeField] private float multiplier = 1.6f;
    [SerializeField] private float duration = 5f;

    protected override void ApplyPowerUp(PlayerController player)
    {
        player.ApplyAbility(new SpeedBoostAbility(multiplier), duration);
    }
}