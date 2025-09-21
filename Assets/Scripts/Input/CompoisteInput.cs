using UnityEngine;

public class CompoisteInput : MonoBehaviour, InterfacePlayerInput
{
    [SerializeField] private MonoBehaviour[] inputProviders;
    private InterfacePlayerInput[] inputs;

    private void Awake()
    {
        inputs = new InterfacePlayerInput[inputProviders.Length];
        for (int i = 0; i < inputProviders.Length; i++)
        {
            inputs[i] = inputProviders[i] as InterfacePlayerInput;
            if (inputs[i] == null)
            {
                Debug.LogError(inputProviders[i].name + " does not implement InterfacePlayerInput!");
            }
        }
    }

    public float GetHorizontal()
    {
        float h = 0f;
        for (int i = 0; i < inputs.Length; i++)
        {
            h += inputs[i].GetHorizontal();
        }
        return Mathf.Clamp(h, -1f, 1f); // prevent exceeding -1..1 if both pressed
    }
}
