using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject targetObj;

    [Header("Thresholds")]
    [SerializeField] private float startThreshold = 5f;
    [SerializeField] private float catchUpThreshold = 1f;
	[SerializeField] private float offsetBelowCamera = 1f;

    [Header("Camera Speed Settings")]
    [SerializeField] private float baseSpeed = 2f;          // starting speed
    [SerializeField] private float speedStep = 1f;        // how much to increase every interval
    [SerializeField] private float incrementInterval = 10f; // seconds per increment
    // [SerializeField] private float maxSpeed = 10f;          // cap

    private float currentSpeed;
    private float timer;

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
            currentSpeed = currentSpeed + speedStep;
            // currentSpeed = Mathf.Min(currentSpeed + speedStep, maxSpeed);
            timer = 0f;
            AudioManager.Instance?.PlayCameraSpeedUp();
        }
    }

	void LateUpdate()
	{
		if (!targetObj) return;
		Transform target = targetObj.transform;

		float distance = target.position.y - transform.position.y;

		if (target.position.y < startThreshold) return;

		Vector3 desiredPos = (distance > catchUpThreshold)
			? new Vector3(0f, target.position.y, transform.position.z)
			: new Vector3(0f, transform.position.y + currentSpeed * Time.deltaTime, transform.position.z);

		float t = (distance > catchUpThreshold)
			? distance * Time.deltaTime
			: 1f;

		transform.position = Vector3.Lerp(transform.position, desiredPos, t);
		
		// --- Game Over check ---
        float bottomEdge = transform.position.y - Camera.main.orthographicSize - offsetBelowCamera;
        if (target.position.y < bottomEdge)
        {
            GameManager.Instance.GameOver();
        }
    }
}
