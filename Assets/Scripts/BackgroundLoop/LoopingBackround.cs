// using UnityEngine;

// public class LoopingBackground : MonoBehaviour
// {
//     [SerializeField] private float extraSpace = 0f;
//     private float quadHeight;
//     private Camera mainCamera;

//     private void Start()
//     {
//         quadHeight = GetComponent<MeshRenderer>().bounds.size.y;
//         mainCamera = Camera.main;
//     }

//     private void Update()
//     {
//         float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
//         float quadTop = transform.position.y + quadHeight / 2f;

//         if (quadTop < cameraBottom - extraSpace)
//         {
//             transform.position += new Vector3(0, quadHeight * 3f, 0);
//         }
//     }
// }
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [SerializeField] private float extraSpace = 0.1f;
    [SerializeField] private int totalTiles = 3;

    private float quadHeight;
    private Camera mainCamera;

    private void Start()
    {
        quadHeight = GetComponent<MeshRenderer>().bounds.size.y;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
        float quadTop = transform.position.y + quadHeight / 2f;

        if (quadTop < cameraBottom - extraSpace)
        {
            transform.position += new Vector3(0f, quadHeight * totalTiles, 0f);
        }
    }
}
