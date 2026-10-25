using NaughtyAttributes;
using System;
using System.Collections;
using UC;
using UnityEngine;
using UnityEngine.SceneManagement;

// Lives across scenes (DontDestroyOnLoad). Every movie scene has one, so any of them can be played on its own; the
// copies that come with later scenes are dropped. It holds the takes, the actor's few tries to get through the movie,
// and ends takes (isTakeOver holds the gameplay meanwhile; the director's frustration goes to zero, so it's his face
// that shows):
// - Cut: the director's mood ran out. He says "Cut!", a take is lost, and the scene fades out and starts again. With
//   no takes left he says "You're fired!" too, and it's back to the main menu.
// - Scene complete: the take is good. He's delighted and says "Cut! Print it!", and it fades out to the movie's next
//   scene, keeping the takes. After the last one he says "That's a wrap!" too, and it's back to the main menu.
// Going back to the main menu resets the takes; with no menu scene set, the movie starts again (its first scene).
public class GameManager : MonoBehaviour
{
    [Header("Movie")]
    [SerializeField, Scene, Tooltip("The movie's scenes, in order")]
    private string[]    movieScenes;
    [SerializeField, Scene, Tooltip("Where being fired or finishing the movie goes (empty: the movie starts again)")]
    private string      mainMenuScene;

    [Header("Takes")]
    [SerializeField, Min(1)]
    private int         maxTakes = 3;

    [Header("Director's lines ({braces} keep the text untranslated)")]
    [SerializeField, Tooltip("When his mood runs out")]
    private string      cutLine = "{CUT!}";
    [SerializeField, Tooltip("When there are no takes left")]
    private string      firedLine = "{YOU'RE FIRED!}";
    [SerializeField, Tooltip("When a scene is complete")]
    private string      printLine = "{CUT! PRINT IT!}";
    [SerializeField, Tooltip("After the movie's last scene")]
    private string      wrapLine = "{THAT'S A WRAP!}";

    [Header("Timing")]
    [SerializeField, Min(0), Tooltip("Seconds after each line, before what comes next")]
    private float       pauseAfterLine = 1.0f;
    [SerializeField, Min(0.01f)]
    private float       fadeOutTime = 0.75f;

    static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (!_instance) _instance = FindAnyObjectByType<GameManager>();
            return _instance;
        }
    }

    // The take is ending (cut or complete): gameplay is on hold until the next scene is loaded
    public static bool isTakeOver => (_instance != null) && (_instance.takeOver);

    public int takes => maxTakes;
    public int takesLeft { get; private set; }

    bool takeOver;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            takesLeft = maxTakes;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    // The director's mood ran out (the DirectorDriver calls it)
    public void Cut()
    {
        if (takeOver) return;
        StartCoroutine(CutCR());
    }

    // The take is good (the Scene Complete action)
    public void CompleteScene()
    {
        if (takeOver) return;
        StartCoroutine(CompleteSceneCR());
    }

    IEnumerator CutCR()
    {
        takeOver = true;

        if (DirectorDriver.instance != null) DirectorDriver.instance.SetFrustration(0.0f);
        yield return Say(cutLine);

        // The takes display drops a slate
        takesLeft = Mathf.Max(0, takesLeft - 1);
        yield return new WaitForSeconds(pauseAfterLine);

        bool fired = (takesLeft == 0);
        if (fired)
        {
            yield return Say(firedLine);
            yield return new WaitForSeconds(pauseAfterLine);
        }

        yield return FadeAndLoad((fired) ? (MainMenuScene()) : (SceneManager.GetActiveScene().name));
    }

    IEnumerator CompleteSceneCR()
    {
        takeOver = true;

        var driver = DirectorDriver.instance;
        if (driver != null)
        {
            driver.SetFrustration(0.0f);
            driver.SetMood(1.0f);
        }
        yield return Say(printLine);
        yield return new WaitForSeconds(pauseAfterLine);

        int index = (movieScenes != null) ? (Array.IndexOf(movieScenes, SceneManager.GetActiveScene().name)) : (-1);
        bool last = (index < 0) || (index + 1 >= movieScenes.Length);
        if (last)
        {
            yield return Say(wrapLine);
            yield return new WaitForSeconds(pauseAfterLine);
        }

        yield return FadeAndLoad((last) ? (MainMenuScene()) : (movieScenes[index + 1]));
    }

    // Back to the menu, with all the takes again; with no menu yet, the movie's first scene
    string MainMenuScene()
    {
        takesLeft = maxTakes;
        if (!string.IsNullOrEmpty(mainMenuScene)) return mainMenuScene;
        if ((movieScenes != null) && (movieScenes.Length > 0) && (!string.IsNullOrEmpty(movieScenes[0]))) return movieScenes[0];
        return SceneManager.GetActiveScene().name;
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        if (FullscreenFader.hasFader)
        {
            bool faded = false;
            FullscreenFader.FadeOut(fadeOutTime, Color.black, () => faded = true);
            while (!faded) yield return null;
        }

        // The new scene's fader starts black and fades in
        yield return SceneManager.LoadSceneAsync(sceneName);

        takeOver = false;
    }

    IEnumerator Say(string line)
    {
        if ((string.IsNullOrEmpty(line)) || (DirectorSpeech.instance == null)) yield break;

        yield return DirectorSpeech.instance.Say(new[] { line }, true);
    }
}
