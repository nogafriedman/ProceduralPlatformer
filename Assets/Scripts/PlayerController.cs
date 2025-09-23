using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controls the player character, handling input, movement, jumping,
/// collisions with walls and platforms, scoring, and abilities.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public ScoreManager scoreManager;
    private Rigidbody2D rb;
    private bool facingRight = true;

    [Header("Movement")]
    public float moveAcceleration = 360f;
    public float maxSpeed = 5f;
    private float baseMoveAcceleration;
    private float baseMaxSpeed;
    public float MovementMultiplier { get; set; } = 1f;

    [Header("Jumping")]
    private bool jump = false;
    public float jumpImpulse = 500f;
    public float maxJumpImpulse = 1200f;
    public float HorizontalJumpBonus = 100f;
    public float maxHorizontalBonus = 200f;

    [Header("Airtime / Floatiness")]
    public float ascendGravityScale = 0.75f;
    public float fallGravityScale = 1.5f;
    public float jumpSustainTime = 0.12f;
    public float jumpSustainForce = 18f;
    private float sustainTimer;

    [Header("Walls")]
    public float wallBounceMultiplier = 1.25f;
    public float maxBounceSpeed = 8f;

    [Header("GroundCheck")]
    private bool isGrounded;
    private int groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask wallLayers;

    [Header("VFX")]
    [SerializeField] private GameObject jetpackVFX;
    [SerializeField] private GameObject speedBoostVFX;

    [Header("Jetpack")]
    [SerializeField] private float jetpackThrust = 18f;
    [SerializeField] private float jetpackMaxVerticalSpeed = 8f;
    [SerializeField] private float jetpackGravityScale = 0.25f;
    private bool jetpackActive = false;

    [Header("Input Providers")]
    [SerializeField] private MonoBehaviour inputProvider;
    private InterfacePlayerInput input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundMask = LayerMask.GetMask("Ground");
        baseMoveAcceleration = moveAcceleration;
        baseMaxSpeed = maxSpeed;

        if (jetpackVFX) jetpackVFX.SetActive(false);
        if (speedBoostVFX) speedBoostVFX.SetActive(false);

        input = inputProvider as InterfacePlayerInput;
        if (input == null)
        {
            Debug.LogError("Input provider does not implement InterfacePlayerInput!");
        }
    }

    private void Update()
    {
        isGrounded = groundCheck && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
        if (isJumping() && isGrounded)
        {
            jump = true;
            sustainTimer = jumpSustainTime;
        }
    }

    private void FixedUpdate()
    {
        float inputX = input.GetHorizontal();
        float currSpeedX = rb.linearVelocity.x;

        float accel = isGrounded ? moveAcceleration : moveAcceleration * 0.80f;

        float boostedAcceleration = accel * MovementMultiplier;
        float boostedMaxSpeed = maxSpeed * MovementMultiplier;

        if (Mathf.Abs(inputX) > 0.05f)
        {
            rb.AddForce(new Vector2(inputX * boostedAcceleration, 0f));

            if ((inputX > 0 && !facingRight) || (inputX < 0 && facingRight))
            {
                FlipFacing();
            }
        }
        else
        {
            float decel = isGrounded ? 20f : 4f;
            rb.linearVelocity = new Vector2(
                Mathf.MoveTowards(currSpeedX, 0f, decel * Time.fixedDeltaTime),
                rb.linearVelocity.y
            );
        }

        float cap = isGrounded ? boostedMaxSpeed : boostedMaxSpeed * 0.95f;
        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -cap, cap),
            rb.linearVelocity.y
        );

        if (jump)
        {
            float horizontalBonus = Mathf.Min(Mathf.Abs(rb.linearVelocity.x) * HorizontalJumpBonus, maxHorizontalBonus);
            float totalJumpPower = Mathf.Min(jumpImpulse + horizontalBonus, maxJumpImpulse);

            AudioManager.Instance?.PlayJumpByForce(totalJumpPower, jumpImpulse, maxJumpImpulse);

            rb.AddForce(Vector2.up * totalJumpPower, ForceMode2D.Force);
            jump = false;
        }

        if (sustainTimer > 0f && isJumping() && rb.linearVelocity.y > 0f && !jetpackActive)
        {
            rb.AddForce(Vector2.up * jumpSustainForce, ForceMode2D.Force);
            sustainTimer -= Time.fixedDeltaTime;
        }

        if (jetpackActive)
        {
            rb.gravityScale = jetpackGravityScale;

            if (isJumping() && rb.linearVelocity.y < jetpackMaxVerticalSpeed)
            {
                rb.AddForce(Vector2.up * jetpackThrust, ForceMode2D.Force);
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0f)
            {
                rb.gravityScale = isJumping() ? ascendGravityScale : fallGravityScale;
            }
            else if (rb.linearVelocity.y < 0f)
            {
                rb.gravityScale = fallGravityScale;
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }
    }

    private void FlipFacing()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInLayerMask(collision.gameObject, wallLayers))
        {
            Vector2 v = new Vector2(rb.linearVelocity.x * wallBounceMultiplier, 0f);
            rb.AddForce(v, ForceMode2D.Impulse);
        }

        if (IsInLayerMask(collision.gameObject, groundLayers) &&
            collision.gameObject.TryGetComponent<PlatformIndex>(out var p) &&
            isGrounded)
        {
            int idx = (int)p.floorIndex;
            scoreManager.UpdateState(idx);
        }
    }

    // private void OnTriggerEnter2D(Collider2D col)
    // {
    //     if (col.TryGetComponent<IAbility>(out var ability))
    //     {
    //         Debug.Log("Picked up: " + ability.GetType().Name);

    //         ApplyAbility(ability, 5f);

    //         Destroy(col.gameObject);
    //     }
    // }

    public IEnumerator DeactivateAfter(IAbility ability, float duration)
    {
        yield return new WaitForSeconds(duration);
        ability.Deactivate(this);
    }


    public void ApplyAbility(IAbility ability, float duration)
    {
        ability.Activate(this, duration);
    }

    public void EnableJetpack(bool enable)
    {
        jetpackActive = enable;

        if (jetpackVFX)
        {
            if (enable)
            {
                jetpackVFX.SetActive(true);
                foreach (var ps in jetpackVFX.GetComponentsInChildren<ParticleSystem>())
                    ps.Play();
            }
            else
            {
                foreach (var ps in jetpackVFX.GetComponentsInChildren<ParticleSystem>())
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                jetpackVFX.SetActive(false);
            }
        }
    }

    public void EnableSpeedBoostVFX(bool enable)
    {
        if (speedBoostVFX == null) return;
        speedBoostVFX.SetActive(enable);
    }

    private bool isJumping()
    {
        if (Input.GetButtonDown("Jump"))
        {
            return true;
        }

        if (EventSystem.current == null) return false;

        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        bool isPointerOverUI_Touch = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
            {
                isPointerOverUI_Touch = true;
                break;
            }
        }

        if ((Input.touchCount > 0 && !isPointerOverUI_Touch) || (Input.GetMouseButtonDown(0) && !isPointerOverUI))
        {
            return true;
        }

        return false;
    }
}
