/// <summary>
/// Ability that enables jetpack flight for the player.
/// While active, reduces gravity and applies thrust when jumping.
/// </summary>
public class JetpackAbility : IAbility
{
    public bool IsActive { get; private set; }

    public void Activate(PlayerController player, float duration)
    {
        if (IsActive) return;
        IsActive = true;
        player.EnableJetpack(true);
    }

    public void Deactivate(PlayerController player)
    {
        if (!IsActive) return;
        IsActive = false;
        player.EnableJetpack(false);
    }
}
