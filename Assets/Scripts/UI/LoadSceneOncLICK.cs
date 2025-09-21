using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public void Load() => SceneManager.LoadScene(sceneName);
}