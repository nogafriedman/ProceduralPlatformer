using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;    

    [Header("Thresholds")]
    [SerializeField] private float startThreshold = 5f;
    [SerializeField] private float catchUpThreshold = 1f;
    [SerializeField] private float offsetBelowCamera = 1f;

    [Header("Camera Speed Settings")]
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float speedStep = 1f;
    [SerializeField] private float incrementInterval = 10f;

    private float currentSpeed;
    private float timer;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        currentSpeed = baseSpeed;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= incrementInterval)
        {
            currentSpeed += speedStep;
            timer = 0f;

            AudioManager.Instance?.PlayCameraSpeedUp();
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        float distance = target.position.y - transform.position.y;

        if (target.position.y < startThreshold) return;

        Vector3 desiredPos = (distance > catchUpThreshold)
            ? new Vector3(0f, target.position.y, transform.position.z)
            : new Vector3(0f, transform.position.y + currentSpeed * Time.deltaTime, transform.position.z);

        float t = (distance > catchUpThreshold)
            ? distance * Time.deltaTime
            : 1f;

        transform.position = Vector3.Lerp(transform.position, desiredPos, t);

        float bottomEdge = transform.position.y - cam.orthographicSize - offsetBelowCamera;
        if (target.position.y < bottomEdge)
        {
            GameManager.Instance.GameOver();
        }
    }
}