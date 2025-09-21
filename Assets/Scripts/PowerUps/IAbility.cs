public interface IAbility
{
    void Activate(PlayerController player, float duration);
    void Deactivate(PlayerController player);
    bool IsActive { get; }
}