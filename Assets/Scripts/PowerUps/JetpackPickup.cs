using UnityEngine;

/// <summary>
/// Pickup component for the jetpack power-up.
/// Creates a JetpackAbility and applies it to the player
/// for the configured duration.
/// </summary>
public class JetpackPickup : PowerUpPickup
{
    [SerializeField] private float duration = 8f;

    protected override void ApplyPowerUp(PlayerController player)
    {
        player.ApplyAbility(new JetpackAbility(), duration);
    }
}
