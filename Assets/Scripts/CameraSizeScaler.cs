using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSizeScaler : MonoBehaviour
{
    private Camera cam;

    [Header("Target Sizes")]
    public float sizeAt16x9 = 8f;
    public float sizeAt4x3 = 6.3f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true; // make sure camera is orthographic
    }

    void Update()
    {
        // Current aspect ratio
        float aspect = (float)Screen.width / Screen.height;

        // Reference aspect ratios
        float aspect16x9 = 16f / 9f; // ~1.777
        float aspect4x3 = 4f / 3f;   // ~1.333

        // Interpolation factor (no clamping, so it extrapolates)
        float t = (aspect - aspect4x3) / (aspect16x9 - aspect4x3);

        // Interpolate (and extrapolate if out of range)
        cam.orthographicSize = Mathf.Lerp(sizeAt4x3, sizeAt16x9, t);
    }
}
