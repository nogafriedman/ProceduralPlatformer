using UnityEngine;

public class SpeedBoostPickup : PowerUpPickup
{
    [SerializeField] private float multiplier = 1.6f;
    [SerializeField] private float duration = 5f;

    protected override void ApplyPowerUp(PlayerController player)
    {
        player.ApplyAbility(new SpeedBoostAbility(multiplier), duration);
    }
}