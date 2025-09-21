using UnityEngine;

public class MovingEffect : MonoBehaviour, IPlatformEffect
{
    public PlatformTypeSO data;

    private Vector3 startPos;
    private int direction = 1;
    private float speed;
    private float range;

    private void Awake()
    {
        startPos = transform.position;

        speed = data != null ? data.moveSpeed : 2f;
        range = data != null ? data.moveRange : 3f;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * direction * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= range)
        {
            direction *= -1;
            float clampedX = Mathf.Clamp(transform.position.x, startPos.x - range, startPos.x + range);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }

    public void OnPlayerLanded(Rigidbody2D rb)
    {
    }

    public void Setup()
    {
        // nothing yet
    }
}
