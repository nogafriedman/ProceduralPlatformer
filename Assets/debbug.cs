using TMPro;
using UnityEngine;

public class debbug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        debugText.text = "Width: " + Screen.width + "\nHeight: " + Screen.height + "\nOrthographic Size: " + Camera.main.orthographicSize + "\nField of View: " + Camera.main.fieldOfView;

    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = "\nPlayer Position: " + player.position.ToString("F2");

    }
}
