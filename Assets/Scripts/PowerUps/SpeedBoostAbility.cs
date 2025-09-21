public class SpeedBoostAbility : IAbility
{
    private readonly float _multiplier;
    private bool _active;

    public SpeedBoostAbility(float multiplier)
    {
        _multiplier = multiplier;
    }

    public bool IsActive => _active;

    public void Activate(PlayerController player, float duration)
    {
        if (_active) return;

        _active = true;
        player.MovementMultiplier *= _multiplier;
        player.EnableSpeedBoostVFX(true);
        player.StartCoroutine(player.DeactivateAfter(this, duration));
    }

    public void Deactivate(PlayerController player)
    {
        if (!_active) return;

        _active = false;
        player.MovementMultiplier /= _multiplier;
        player.EnableSpeedBoostVFX(false);
    }
}
