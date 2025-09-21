using UnityEngine;

public class LoopingBackgroundGroup : MonoBehaviour
{
    private void Start()
    {
        var tiles = GetComponentsInChildren<LoopingBackground>();
        if (tiles.Length < 2) return;

        float quadHeight = tiles[0].GetComponent<MeshRenderer>().bounds.size.y;

        for (int i = 0; i < tiles.Length; i++)
        {
            Vector3 originalPos = tiles[i].transform.localPosition;
            float offsetY = (i - (tiles.Length / 2f)) * quadHeight;
            tiles[i].transform.localPosition = new Vector3(originalPos.x, offsetY, originalPos.z);
        }

    }
}
