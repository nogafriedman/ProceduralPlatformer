using UnityEngine;

public enum PlatformEffectType
{
    None,
    Bouncy,
    Sticky,
    Moving
}

[CreateAssetMenu(menuName = "Platforms/Platform Type")]
public class PlatformTypeSO : ScriptableObject
{
    [Header("Identification")]
    public string typeName;

    [Header("Visuals")]
    public Sprite sprite;

    [Header("Effect Type")]
    public PlatformEffectType effectType = PlatformEffectType.None;

    [Header("Gameplay Parameters")]
    public float jumpForce;
    public float stickyDuration;
    public float moveSpeed;
    public float moveRange;


}