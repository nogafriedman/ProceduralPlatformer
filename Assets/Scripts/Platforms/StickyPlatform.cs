using UnityEngine;

public class StickyEffect : MonoBehaviour, IPlatformEffect
{
    public PlatformTypeSO data; 

    private Rigidbody2D stickyRb;
    private float stickyTimer;
    private PlayerController playerController;

    public void Setup()
    {
        stickyRb = null;
        stickyTimer = 0f;
        playerController = null;
    }

    public void OnPlayerLanded(Rigidbody2D rb)
    {
        if (stickyRb != null) return;

        // Stick only if the player lands on top
        float playerBottom = rb.transform.position.y - rb.GetComponent<Collider2D>().bounds.extents.y;
        float platformTop = transform.position.y + GetComponent<Collider2D>().bounds.extents.y;

        if (playerBottom >= platformTop - 0.05f)
        {
            stickyRb = rb;
            stickyTimer = data.stickyDuration;

            playerController = rb.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.enabled = false;

            // Freeze player movement
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
    }

    private void Update()
    {

        if (this == null) return;

        if (stickyRb != null)
        {
            stickyTimer -= Time.deltaTime;
            if (stickyTimer <= 0f)
                Unstick();
        }
    }

    private void Unstick()
    {
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController = null;
        }

        if (stickyRb != null)
        {
            stickyRb.gravityScale = 1f;
            stickyRb = null;
        }
    }
}
