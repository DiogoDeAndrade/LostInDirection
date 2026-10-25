using NaughtyAttributes;
using System.Collections;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// The title screen, over its own copy of the director's set, with its music. The menu is UnityCommon's gamepad menu
// (Options, Credits and Quit are its own button events; Back leaves the options too, not only their Back button).
// Play fades the menu out, claps the slate in front of the director, then fades out to the movie's first scene. While
// the credits roll, the title is hidden and the set dimmed. Meanwhile the director is happy and now and then gestures
// as if he were explaining something.
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private UIButton        playButton;
    [SerializeField, Tooltip("The menu's buttons: switched off once Play is pressed, and while the credits roll")]
    private UIGroup         menuGroup;
    [SerializeField, Tooltip("The whole menu, faded out once Play is pressed")]
    private CanvasGroup     menuCanvasGroup;
    [SerializeField, Tooltip("Hidden while the credits roll (the title, ...)")]
    private CanvasGroup[]   hideDuringCredits;
    [SerializeField, Tooltip("A dark full-screen overlay, shown behind the credits")]
    private CanvasGroup     creditsDim;
    [SerializeField, Tooltip("The credits' own group: they're on while it shows")]
    private CanvasGroup     creditsGroup;
    [SerializeField, Tooltip("The options panel: Back (UIBack) presses its back button while it's up")]
    private UIGroup         optionsGroup;
    [SerializeField]
    private UIButton        optionsBack;
    [SerializeField]
    private PlayerInput     playerInput;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton]
    private UC.InputControl backControl;
    [SerializeField]
    private AudioClip       music;
    [SerializeField, Range(0.0f, 1.0f)]
    private float           creditsDimAlpha = 0.6f;
    [SerializeField, Scene]
    private string          firstScene = "ZombieMovie";
    [SerializeField, Min(0.01f)]
    private float           fadeTime = 0.5f;
    [SerializeField, MinMaxSlider(0.0f, 20.0f), Tooltip("Seconds between the director's gestures")]
    private Vector2     gestureInterval = new Vector2(4.0f, 8.0f);
    [SerializeField, MinMaxSlider(0.0f, 10.0f), Tooltip("How long each lasts")]
    private Vector2     gestureDuration = new Vector2(1.0f, 2.5f);

    bool    starting;

    void Start()
    {
        playButton.onInteract += Play;
        if (backControl != null) backControl.playerInput = playerInput;
        StartCoroutine(GestureCR());
        if (music != null) SoundManager.PlayMusic(music);
    }

    void OnDestroy()
    {
        if (playButton != null) playButton.onInteract -= Play;
    }

    // The credits are on while their group shows (not while the options are up)
    void Update()
    {
        if ((optionsGroup != null) && (optionsGroup.uiEnable) && (optionsBack != null) && (backControl != null) && (backControl.IsDown()))
        {
            optionsBack.Interact();
        }

        bool credits = (!starting) && (creditsGroup != null) && (creditsGroup.alpha > 0.01f);
        float step = Time.deltaTime / fadeTime;

        if (creditsDim != null) creditsDim.alpha = Mathf.MoveTowards(creditsDim.alpha, (credits) ? (creditsDimAlpha) : (0.0f), step);
        if (hideDuringCredits != null)
        {
            foreach (var group in hideDuringCredits)
            {
                if (group != null) group.alpha = Mathf.MoveTowards(group.alpha, (credits) ? (0.0f) : (1.0f), step);
            }
        }
    }

    void Play(BaseUIControl control)
    {
        if (starting) return;
        starting = true;
        StartCoroutine(PlayCR());
    }

    IEnumerator PlayCR()
    {
        if (menuGroup != null) menuGroup.EnableUI(false);
        if (menuCanvasGroup != null) menuCanvasGroup.FadeOut(fadeTime);
        if (Director.instance != null) Director.instance.talking = false;

        if (Slate.instance != null) yield return Slate.instance.Play();

        FullscreenFader.FadeOut(fadeTime, Color.black, () => SceneManager.LoadScene(firstScene));
    }

    IEnumerator GestureCR()
    {
        while (!starting)
        {
            yield return new WaitForSeconds(Random.Range(gestureInterval.x, gestureInterval.y));
            if ((starting) || (Director.instance == null)) continue;

            Director.instance.talking = true;
            yield return new WaitForSeconds(Random.Range(gestureDuration.x, gestureDuration.y));
            if (Director.instance != null) Director.instance.talking = false;
        }
    }
}
