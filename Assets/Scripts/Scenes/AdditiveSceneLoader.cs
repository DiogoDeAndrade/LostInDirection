using UnityEngine;
using UnityEngine.SceneManagement;

// Loads other scenes on top of this one (e.g. the Director scene, shared by every movie), unless they're already
// open (multi-scene editing). The scenes have to be in the build list.
public class AdditiveSceneLoader : MonoBehaviour
{
    [SerializeField]
    private string[]    scenes = { "Director" };

    void Awake()
    {
        foreach (var sceneName in scenes)
        {
            if (string.IsNullOrEmpty(sceneName)) continue;
            if (SceneManager.GetSceneByName(sceneName).isLoaded) continue;

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
