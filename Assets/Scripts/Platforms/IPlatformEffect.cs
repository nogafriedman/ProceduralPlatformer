using UnityEngine;

public interface IPlatformEffect
{
    void OnPlayerLanded(Rigidbody2D rb);
    void Setup();
}
