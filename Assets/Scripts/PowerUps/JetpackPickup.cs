using UnityEngine;

public class JetpackPickup : PowerUpPickup
{
    [SerializeField] private float duration = 8f;

    protected override void ApplyPowerUp(PlayerController player)
    {
        player.ApplyAbility(new JetpackAbility(), duration);
    }
}
