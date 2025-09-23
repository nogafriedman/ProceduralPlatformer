/// <summary>
/// Defines the contract for abilities that can be applied to the player.
/// Each ability has an activate phase, a deactivate phase,
/// and an active state.
/// </summary>
public interface IAbility
{
     /// <summary>Activates the ability on the player for a given duration.</summary>
    void Activate(PlayerController player, float duration);

    /// <summary>Deactivates the ability from the player.</summary>
    void Deactivate(PlayerController player);

    /// <summary>Indicates whether the ability is currently active.</summary>
    bool IsActive { get; }
}