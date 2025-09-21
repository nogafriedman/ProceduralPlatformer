using UnityEngine;

public class ConfettiCameraFollow : MonoBehaviour
{
    void LateUpdate()
    {
        float bottomY = Camera.main.transform.position.y - Camera.main.orthographicSize;
        transform.position = new Vector3(Camera.main.transform.position.x, bottomY, 0f);
    }
}
