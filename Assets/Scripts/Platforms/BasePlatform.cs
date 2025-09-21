using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BasePlatform : MonoBehaviour
{
    private List<IPlatformEffect> effects = new List<IPlatformEffect>();
    private SpriteRenderer sr;
    private int playerLayer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        playerLayer = LayerMask.NameToLayer("Player");
        SetupEffects(); // <-- Add this line here
    }

    private void SetupEffects()
    {
        effects.Clear();
        foreach (var comp in GetComponents<IPlatformEffect>())
        {
            if (comp != null)
                effects.Add(comp);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != playerLayer) return;
        if (collision.rigidbody == null) return;

        foreach (var effect in effects)
        {
            if (effect == null) continue;
            effect.OnPlayerLanded(collision.rigidbody);
        }
    }
}