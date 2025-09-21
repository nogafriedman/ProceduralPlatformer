using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [Header("Reference Resolution")]
    [Min(1)] [SerializeField] private int referenceWidth = 720;
    [Min(1)] [SerializeField] private int referenceHeight = 1600;
    [Min(1)] [SerializeField] private float pixelsPerUnit = 100f;

    [Header("Scaling Mode")]
    public ScaleMode scaleMode = ScaleMode.FitHeight;

    [Header("Interpolated Settings (only for Interpolated mode)")]
    public float sizeAt16x9 = 8f;
    public float sizeAt4x3 = 6.3f;

    [Header("Safe Area")]
    public bool respectSafeArea = true;

    private Camera cam;
    private int lastW, lastH;
    private Rect lastSafe;

    public enum ScaleMode
    {
        FitHeight,
        FitWidth,
        Fill,
        ContainWithBars,
        Interpolated
    }

    private float DesignOrthoSize => referenceHeight / (2f * pixelsPerUnit);

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        Apply(true);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        cam.orthographic = true;
        Apply(true);
    }
#endif

    private void LateUpdate()
    {
        Rect sa = GetSafeArea();
        if (Screen.width != lastW || Screen.height != lastH || sa != lastSafe)
            Apply();
    }

    private void Apply(bool force = false)
    {
        if (!cam) cam = GetComponent<Camera>();
        cam.orthographic = true;

        Rect safe = GetSafeArea();
        lastW = Screen.width;
        lastH = Screen.height;
        lastSafe = safe;

        Rect baseRect = respectSafeArea ? Normalize(safe) : new Rect(0, 0, 1, 1);

        float usedW = Screen.width * baseRect.width;
        float usedH = Screen.height * baseRect.height;
        float targetAspect = usedW / usedH;
        float designAspect = (float)referenceWidth / referenceHeight;

        float ortho = DesignOrthoSize;
        Rect viewport = baseRect;

        switch (scaleMode)
        {
            case ScaleMode.FitHeight:
                ortho = DesignOrthoSize;
                break;

            case ScaleMode.FitWidth:
                ortho = referenceWidth / (2f * pixelsPerUnit * targetAspect);
                break;

            case ScaleMode.Fill:
                ortho = (targetAspect > designAspect)
                    ? referenceWidth / (2f * pixelsPerUnit * targetAspect)
                    : DesignOrthoSize;
                break;

            case ScaleMode.ContainWithBars:
                if (targetAspect > designAspect)
                {
                    // Pillar-box
                    ortho = DesignOrthoSize;
                    float w01 = designAspect / targetAspect;
                    viewport = new Rect(
                        baseRect.x + baseRect.width * (1f - w01) * 0.5f,
                        baseRect.y,
                        baseRect.width * w01,
                        baseRect.height
                    );
                }
                else
                {
                    // Letter-box
                    ortho = referenceWidth / (2f * pixelsPerUnit * targetAspect);
                    float h01 = targetAspect / designAspect;
                    viewport = new Rect(
                        baseRect.x,
                        baseRect.y + baseRect.height * (1f - h01) * 0.5f,
                        baseRect.width,
                        baseRect.height * h01
                    );
                }
                break;

            case ScaleMode.Interpolated:
                float aspect = (float)Screen.width / Screen.height;
                float aspect16x9 = 16f / 9f;
                float aspect4x3 = 4f / 3f;
                float t = (aspect - aspect4x3) / (aspect16x9 - aspect4x3);
                ortho = Mathf.Lerp(sizeAt4x3, sizeAt16x9, t);
                break;
        }

        cam.rect = viewport;
        cam.orthographicSize = Mathf.Max(0.0001f, ortho);
    }

    private static Rect Normalize(Rect r)
    {
        if (Screen.width <= 0 || Screen.height <= 0) return new Rect(0, 0, 1, 1);
        return new Rect(r.x / Screen.width, r.y / Screen.height, r.width / Screen.width, r.height / Screen.height);
    }

    private static Rect GetSafeArea()
    {
#if UNITY_EDITOR
        return new Rect(0, 0, Screen.width, Screen.height);
#else
        return Screen.safeArea;
#endif
    }
}
