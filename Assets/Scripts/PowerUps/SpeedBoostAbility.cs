using UnityEngine;
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
        _active = true;
        player.MovementMultiplier *= _multiplier;
        player.StartCoroutine(DeactivateAfter(player, duration));
    }

    public void Deactivate(PlayerController player)
    {
        _active = false;
        player.MovementMultiplier /= _multiplier;
    }

    private System.Collections.IEnumerator DeactivateAfter(PlayerController player, float duration)
    {
        yield return new WaitForSeconds(duration);
        Deactivate(player);
    }
}
