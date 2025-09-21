using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class SpriteAndColliderFitter : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] public Vector2 targetSpriteSize = new Vector2(3f, 1f); // desired width x height in world units

    [Header("Physics")]
    [SerializeField] private float colliderHeight = 0.2f; // thin strip height

    private SpriteRenderer sr;
    private BoxCollider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        //Fit();
    }

    private void OnValidate()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (col == null) col = GetComponent<BoxCollider2D>();
        //Fit();
    }

    public void Fit()
    {
        if (sr.sprite == null) return;

        // Scale sprite so it reaches the desired size in world units
        Vector2 spriteSize = sr.sprite.bounds.size;
        Vector3 newScale = transform.localScale;

        newScale.x = targetSpriteSize.x / spriteSize.x;
        newScale.y = targetSpriteSize.y / spriteSize.y;

        transform.localScale = newScale;

        // Collider: full width, thin strip at the top
        col.size = new Vector2(targetSpriteSize.x, colliderHeight);
        col.offset = new Vector2(0, (targetSpriteSize.y - colliderHeight) / 2f);
    }
}
