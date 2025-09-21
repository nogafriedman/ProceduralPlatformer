using UnityEngine;

public class JetpackAbility : IAbility
{
    private bool _active;

    public bool IsActive => _active;

    public void Activate(PlayerController player, float duration)
    {
        _active = true;
        player.EnableJetpack(true);
        player.StartCoroutine(DeactivateAfter(player, duration));
    }

    public void Deactivate(PlayerController player)
    {
        _active = false;
        player.EnableJetpack(false);
    }

    private System.Collections.IEnumerator DeactivateAfter(PlayerController player, float duration)
    {
        yield return new WaitForSeconds(duration);
        Deactivate(player);
    }
}