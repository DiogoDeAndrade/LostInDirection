using NaughtyAttributes;
using System.Collections;
using UC;
using UnityEngine;

// The clapperboard in the Director scene, waiting out of the director camera's view. Play slides it to the shown
// point, claps it (the Animator's clap trigger, with clapSound when the arm shuts) and slides it back out, then
// resets it to idle. While that runs isPlaying is true, which holds the gameplay (see Gameplay). One per Director
// scene.
public class Slate : MonoBehaviour
{
    [SerializeField, Tooltip("The first one in the children if left empty")]
    private Animator    animator;
    [SerializeField, AnimatorParam(nameof(animator), AnimatorControllerParameterType.Trigger)]
    private string      clapTrigger = "Clap";
    [SerializeField, Tooltip("State it's reset to once it's gone")]
    private string      idleState = "Idle";
    [SerializeField, Tooltip("Where it's seen clapping (it waits where it starts)")]
    private Transform   shownPoint;
    [SerializeField, Min(0.01f), Tooltip("Seconds to slide in, and to slide out")]
    private float       moveTime = 0.4f;
    [SerializeField, Min(0)]
    private float       holdBeforeClap = 0.3f;
    [SerializeField, Min(0), Tooltip("Seconds from the trigger to the end of the clap")]
    private float       clapTime = 0.55f;
    [SerializeField, Min(0)]
    private float       holdAfterClap = 0.4f;
    [SerializeField, Tooltip("Played on the clap (optional)")]
    private SoundDef    clapSound;
    [SerializeField, Min(0), Tooltip("Seconds from the trigger to the arm shutting (the clip shuts 0.92 s in, played at 4x)")]
    private float       clapSoundDelay = 0.23f;

    public static Slate instance { get; private set; }

    public static bool isPlaying => (instance != null) && (instance.playing);

    Vector3 hiddenPosition;
    bool    playing;

    void Awake()
    {
        instance = this;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        hiddenPosition = transform.position;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public IEnumerator Play()
    {
        if (shownPoint == null)
        {
            Debug.LogWarning("Slate: no shown point", this);
            yield break;
        }

        playing = true;

        yield return Move(hiddenPosition, shownPoint.position, true);
        yield return new WaitForSeconds(holdBeforeClap);

        if (animator != null) animator.SetTrigger(clapTrigger);
        yield return new WaitForSeconds(clapSoundDelay);
        if (clapSound != null) clapSound.Play();
        yield return new WaitForSeconds(Mathf.Max(0.0f, clapTime + holdAfterClap - clapSoundDelay));

        yield return Move(shownPoint.position, hiddenPosition, false);
        if (animator != null)
        {
            animator.ResetTrigger(clapTrigger);
            animator.Play(idleState, 0, 0.0f);
        }

        playing = false;
    }

    // Eases out when coming in (it arrives gently) and in when leaving (it whips away)
    IEnumerator Move(Vector3 from, Vector3 to, bool easeOut)
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Mathf.Min(1.0f, t + Time.deltaTime / moveTime);
            float k = (easeOut) ? (1.0f - (1.0f - t) * (1.0f - t)) : (t * t);
            transform.position = Vector3.Lerp(from, to, k);
            yield return null;
        }
    }
}
