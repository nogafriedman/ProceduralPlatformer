using UnityEngine;

public class KeyboardInput : MonoBehaviour, InterfacePlayerInput
{
    public float GetHorizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }
}
