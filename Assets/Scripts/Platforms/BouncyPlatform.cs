using UnityEngine;

public class BouncyEffect : MonoBehaviour, IPlatformEffect
{
    public PlatformTypeSO data;

    public void Setup() { }

    public void OnPlayerLanded(Rigidbody2D rb)
    {
        if (rb.linearVelocity.y <= 0f)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = data.jumpForce;
            rb.linearVelocity = velocity;
        }
    }
}
