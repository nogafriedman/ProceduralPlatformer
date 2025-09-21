public class JetpackAbility : IAbility
{
    private bool _active;

    public bool IsActive => _active;

    public void Activate(PlayerController player, float duration)
    {
        if (_active) return;

        _active = true;
        player.EnableJetpack(true);
        player.StartCoroutine(player.DeactivateAfter(this, duration));
    }

    public void Deactivate(PlayerController player)
    {
        if (!_active) return;

        _active = false;
        player.EnableJetpack(false);
    }
}
