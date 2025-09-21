using UnityEngine;

public class UIButtonInput : MonoBehaviour, InterfacePlayerInput
{
    private float horizontal;

    public void OnLeftButtonDown()
    {
        horizontal = -1f;
    }

    public void OnLeftButtonUp()
    {
        horizontal = 0f;
    }

    public void OnRightButtonDown()
    {
        horizontal = 1f;
    }

    public void OnRightButtonUp()
    {
        horizontal = 0f;
    }

    public float GetHorizontal()
    {
        return horizontal;
    }
}
