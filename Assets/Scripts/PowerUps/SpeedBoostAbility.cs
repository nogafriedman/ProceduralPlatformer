/// <summary>
/// Ability that increases the player's horizontal movement speed
/// by a given multiplier for a limited duration.
/// </summary>
public class SpeedBoostAbility : IAbility
{
    private readonly float multiplier;
    public bool IsActive { get; private set; }

    public SpeedBoostAbility(float multiplier)
    {
        this.multiplier = multiplier;
    }

    public void Activate(PlayerController player, float duration)
    {
        if (IsActive) return;
        IsActive = true;
        player.MovementMultiplier *= multiplier;
        player.EnableSpeedBoostVFX(true);
    }

    public void Deactivate(PlayerController player)
    {
        if (!IsActive) return;
        IsActive = false;
        player.MovementMultiplier /= multiplier;
        player.EnableSpeedBoostVFX(false);
    }
}

