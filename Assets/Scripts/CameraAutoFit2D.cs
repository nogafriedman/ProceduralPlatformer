using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraAutoFit2D : MonoBehaviour
{
    [Min(1)] public int referenceWidth = 720;
    [Min(1)] public int referenceHeight = 1600;
    [Min(1)] public float pixelsPerUnit = 100f;

    public enum FitMode { FitHeight, FitWidth, Fill, ContainWithBars }
    public FitMode fit = FitMode.FitHeight;

    public bool respectSafeArea = true;

    private Camera cam;
    private int lastW, lastH;
    private Rect lastSafe;

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

        Rect r = baseRect;
        float ortho;

        switch (fit)
        {
            case FitMode.FitHeight:
                ortho = DesignOrthoSize;
                break;

            case FitMode.FitWidth:
                ortho = referenceWidth / (2f * pixelsPerUnit * targetAspect);
                break;

            case FitMode.Fill:
                ortho = (targetAspect > designAspect)
                    ? referenceWidth / (2f * pixelsPerUnit * targetAspect)
                    : DesignOrthoSize;
                break;

            case FitMode.ContainWithBars:
                if (targetAspect > designAspect)
                {
                    // Pillar-box inside baseRect
                    ortho = DesignOrthoSize;
                    float w01 = designAspect / targetAspect;
                    r = new Rect(
                        baseRect.x + baseRect.width * (1f - w01) * 0.5f,
                        baseRect.y,
                        baseRect.width * w01,
                        baseRect.height
                    );
                }
                else
                {
                    // Letter-box inside baseRect
                    ortho = referenceWidth / (2f * pixelsPerUnit * targetAspect);
                    float h01 = targetAspect / designAspect;
                    r = new Rect(
                        baseRect.x,
                        baseRect.y + baseRect.height * (1f - h01) * 0.5f,
                        baseRect.width,
                        baseRect.height * h01
                    );
                }
                break;

            default:
                ortho = DesignOrthoSize;
                break;
        }

        cam.rect = r;
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
