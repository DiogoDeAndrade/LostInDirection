using NaughtyAttributes;
using System.Collections;
using UC;
using UnityEngine;

// Cutscene mode, started and ended by verb actions. Begin slides black bars in at the top and bottom of the screen,
// fades the gameplay UI out and can switch to another camera; while it's on, the actor's controls are locked (Player
// checks isActive). End switches the camera back, slides the bars out and fades the UI back in.
// One per scene, on its own canvas above the gameplay UI. The bars (children of this object) are collapsed to zero
// height outside cutscenes; the inspector buttons show them for editing.
public class Cutscene : MonoBehaviour
{
    [SerializeField]
    private RectTransform   topBar;
    [SerializeField]
    private RectTransform   bottomBar;
    [SerializeField, Range(0.0f, 0.5f), Tooltip("Height of each bar, as a fraction of the screen height")]
    private float           barSize = 0.12f;
    [SerializeField, Min(0), Tooltip("Seconds the bars take to slide in or out, and the UI to fade")]
    private float           transitionTime = 0.5f;
    [SerializeField, Tooltip("Faded out while a cutscene plays (the gameplay UI)")]
    private CanvasGroup[]   fadeDuringCutscene;

    public static Cutscene instance { get; private set; }

    // A cutscene is playing (from the start of Begin to the end of End)
    public static bool isActive => (instance != null) && (instance.active);

    bool    active;
    float   barAmount;
    Camera  cutsceneCamera;
    Camera  gameplayCamera;

    void Awake()
    {
        instance = this;
        SetBars(0.0f);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    // Starts a cutscene, viewed through camera (left as it is when null); finishes when the bars are in
    public IEnumerator Begin(Camera camera)
    {
        if (active) yield break;
        active = true;

        foreach (var group in fadeDuringCutscene)
        {
            if (group != null) group.FadeOut(transitionTime);
        }

        if (camera != null)
        {
            gameplayCamera = Camera.main;
            cutsceneCamera = camera;
            cutsceneCamera.gameObject.SetActive(true);
            cutsceneCamera.enabled = true;
            if ((gameplayCamera != null) && (gameplayCamera != cutsceneCamera)) gameplayCamera.enabled = false;
        }

        yield return MoveBars(1.0f);
    }

    // Ends the cutscene; the actor gets control back once the bars are out
    public IEnumerator End()
    {
        if (!active) yield break;

        if (cutsceneCamera != null)
        {
            if (gameplayCamera != null) gameplayCamera.enabled = true;
            cutsceneCamera.gameObject.SetActive(false);
            cutsceneCamera = null;
            gameplayCamera = null;
        }

        foreach (var group in fadeDuringCutscene)
        {
            if (group != null) group.FadeIn(transitionTime);
        }

        yield return MoveBars(0.0f);

        active = false;
    }

    [Button("Show bars (edit)")]
    void ShowBarsInEditor() => SetBarsInEditor(1.0f);

    [Button("Hide bars (edit)")]
    void HideBarsInEditor() => SetBarsInEditor(0.0f);

    void SetBarsInEditor(float amount)
    {
        SetBars(amount);
#if UNITY_EDITOR
        if (topBar != null) UnityEditor.EditorUtility.SetDirty(topBar);
        if (bottomBar != null) UnityEditor.EditorUtility.SetDirty(bottomBar);
#endif
    }

    IEnumerator MoveBars(float target)
    {
        float speed = (transitionTime > 0.0f) ? (1.0f / transitionTime) : (float.MaxValue);
        while (barAmount != target)
        {
            SetBars(Mathf.MoveTowards(barAmount, target, speed * Time.deltaTime));
            yield return null;
        }
    }

    void SetBars(float amount)
    {
        barAmount = amount;
        float size = barSize * Mathf.SmoothStep(0.0f, 1.0f, amount);

        if (topBar != null)
        {
            topBar.anchorMin = new Vector2(0.0f, 1.0f - size);
            topBar.anchorMax = Vector2.one;
        }
        if (bottomBar != null)
        {
            bottomBar.anchorMin = Vector2.zero;
            bottomBar.anchorMax = new Vector2(1.0f, size);
        }
    }
}
