using System.Collections;
using System.Collections.Generic;
using TMPro;                        
using UnityEngine;

public class SubtitleHUD : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI label; 

    [Header("Timing")]
    [SerializeField] private float holdSeconds = 0.7f; 
    [SerializeField] private float fadeSeconds = 0.25f; 

    [Header("Clip → Text")]
    [SerializeField] private List<Entry> entries = new List<Entry>();
    [System.Serializable]
    public struct Entry
    {
        public AudioClip clip;         
        [TextArea] public string text;   
    }

    private Dictionary<AudioClip, string> map; 
    private CanvasGroup cg;                    

    void Awake()
    {
        if (!label)
        {
            Debug.LogError("SubtitleHUD: assign TextMeshProUGUI label in Inspector.");
            enabled = false; return;               
        }

     
        cg = label.GetComponent<CanvasGroup>() ?? label.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;                               
        label.text = "";                             

        
        map = new Dictionary<AudioClip, string>(entries.Count);
        foreach (var e in entries)
            if (e.clip) map[e.clip] = e.text;        
    }

    void OnEnable()  => AudioManager.SfxPlayed += HandleSfx;
    void OnDisable() => AudioManager.SfxPlayed -= HandleSfx; 

   
    private void HandleSfx(AudioClip clip)
    {
        if (!clip) return;                          
        if (!map.TryGetValue(clip, out var text))   
            return;

        StopAllCoroutines();                       
        StartCoroutine(Show(text));               
    }

    private IEnumerator Show(string text)
    {
        label.text = text;                         

        
        yield return FadeTo(1f, 0.1f);// Fade in quickly (0.1s feels snappy)

       
        yield return new WaitForSeconds(holdSeconds); // Keep fully visible

       
        yield return FadeTo(0f, fadeSeconds); // Fade out
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = cg.alpha;                    
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;           
            cg.alpha = Mathf.Lerp(start, target, t / duration); 
            yield return null;                      
        }
        cg.alpha = target;                          
    }
}