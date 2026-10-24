using UnityEngine;

// The director, in the Director scene that each movie loads additively; his own camera renders him into the
// portrait in the corner of the HUD. He shows two things: frustration (0 = pleased, 1 = losing it), in his body
// language, which is the hot/cold of where the actor is and what he's aiming at; and mood (1 = delighted,
// 0 = furious), on his face, which is the actor's health. The movie's script sets both; what's shown eases
// towards them. Both are sliders in the inspector, to try them out.
[RequireComponent(typeof(DirectorBody))]
[RequireComponent(typeof(DirectorFace))]
public class Director : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float   _frustration = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float   _mood = 1.0f;
    [SerializeField, Min(0.01f), Tooltip("How fast what's shown follows the values (units per second)")]
    private float   easeSpeed = 1.5f;

    public static Director instance { get; private set; }

    DirectorBody    body;
    DirectorFace    face;
    float           shownFrustration;
    float           shownMood;

    public float frustration
    {
        get => _frustration;
        set => _frustration = Mathf.Clamp01(value);
    }

    public float mood
    {
        get => _mood;
        set => _mood = Mathf.Clamp01(value);
    }

    // Talking gestures (DirectorSpeech sets it while a line is being typed)
    public bool talking
    {
        get => (body != null) && (body.talking);
        set { if (body != null) body.talking = value; }
    }

    void Awake()
    {
        instance = this;
        body = GetComponent<DirectorBody>();
        face = GetComponent<DirectorFace>();

        shownFrustration = _frustration;
        shownMood = _mood;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void Update()
    {
        shownFrustration = Mathf.MoveTowards(shownFrustration, _frustration, easeSpeed * Time.deltaTime);
        shownMood = Mathf.MoveTowards(shownMood, _mood, easeSpeed * Time.deltaTime);

        body.frustration = shownFrustration;
        // Mood 1 is the first (best) face, mood 0 the last
        face.SetFace(Mathf.RoundToInt((1.0f - shownMood) * (face.faceCount - 1)));
    }
}
