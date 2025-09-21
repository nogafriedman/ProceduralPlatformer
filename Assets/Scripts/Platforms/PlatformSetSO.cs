using UnityEngine;

[CreateAssetMenu(menuName = "Platforms/Platform Set")]
public class PlatformSetSO : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public PlatformTypeSO type;
        [Range(0f, 1f)] public float weight = 1f;
    }

    [Tooltip("List of platform types with their spawn weights")]
    public Entry[] types;

    public PlatformTypeSO GetRandomType()
    {
        if (types == null || types.Length == 0)
        {
            Debug.LogWarning("[PlatformSetSO] No types defined!");
            return null;
        }

        // Calculate total weight
        float total = 0f;
        foreach (var entry in types)
            total += entry.weight;

        if (total <= 0f)
        {
            Debug.LogWarning("[PlatformSetSO] All weights are zero!");
            return types[0].type;
        }

        float roll = Random.value * total;

        foreach (var entry in types)
        {
            if (roll <= entry.weight)
                return entry.type;
            roll -= entry.weight;
        }

        // Fallback
        return types[types.Length - 1].type;
    }
}
