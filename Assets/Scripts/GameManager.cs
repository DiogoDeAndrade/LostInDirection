using NaughtyAttributes;
using System.Collections;
using UC;
using UnityEngine;
using UnityEngine.SceneManagement;

// Lives across scenes (DontDestroyOnLoad). Every movie scene has one, so any of them can be played on its own; the
// copies that come with later scenes are dropped. It holds the takes: the actor has a few to get through the movie.
// When the director's mood runs out the take is cut: gameplay stops (isCut holds it), the director's frustration
// goes to zero (so it's his angry face that shows) and he says "Cut!", a take is lost, and the scene fades out and
// starts again. With no takes left he says "You're fired!" too, and it's back to the main menu (there's none yet:
// the scene starts again with all the takes).
public class GameManager : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int     maxTakes = 3;
    [SerializeField, Tooltip("What the director says when his mood runs out ({braces} keep the text untranslated)")]
    private string  cutLine = "{CUT!}";
    [SerializeField, Tooltip("What he says when there are no takes left")]
    private string  firedLine = "{YOU'RE FIRED!}";
    [SerializeField, Min(0), Tooltip("Seconds after each line, before what comes next")]
    private float   pauseAfterLine = 1.0f;
    [SerializeField, Min(0.01f)]
    private float   fadeOutTime = 0.75f;
    [SerializeField, Scene, Tooltip("Where being fired goes (empty: the scene starts again with all the takes)")]
    private string  mainMenuScene;

    static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (!_instance) _instance = FindFirstObjectByType<GameManager>();
            return _instance;
        }
    }

    // The take was cut: gameplay is on hold until the next scene is loaded
    public static bool isCut => (_instance != null) && (_instance.cutting);

    public int takes => maxTakes;
    public int takesLeft { get; private set; }

    bool cutting;

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
        if (cutting) return;
        StartCoroutine(CutCR());
    }

    IEnumerator CutCR()
    {
        cutting = true;

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

        if (FullscreenFader.hasFader)
        {
            bool faded = false;
            FullscreenFader.FadeOut(fadeOutTime, Color.black, () => faded = true);
            while (!faded) yield return null;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        if (fired)
        {
            takesLeft = maxTakes;
            if (!string.IsNullOrEmpty(mainMenuScene)) sceneName = mainMenuScene;
        }
        // The new scene's fader starts black and fades in
        yield return SceneManager.LoadSceneAsync(sceneName);

        cutting = false;
    }

    IEnumerator Say(string line)
    {
        if ((string.IsNullOrEmpty(line)) || (DirectorSpeech.instance == null)) yield break;

        yield return DirectorSpeech.instance.Say(new[] { line }, true);
    }
}
