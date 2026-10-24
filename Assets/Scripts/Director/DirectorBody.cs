using UnityEngine;

// The director's body language, done procedurally on the rigid parts of the Kenney blocky character (no animation
// clips), from a single frustration value: 0 is relaxed (breathing, a slow nod), the middle is impatient (leaning
// in, one arm gesturing, the other held out, shaking his head, tapping a foot), 1 is losing it (arms waving over his
// head, hopping, shouting up). Everything blends continuously, and the tempo rises with the frustration.
public class DirectorBody : MonoBehaviour
{
    [SerializeField]
    private Transform   root;
    [SerializeField]
    private Transform   torso;
    [SerializeField]
    private Transform   head;
    [SerializeField]
    private Transform   armLeft;
    [SerializeField]
    private Transform   armRight;
    [SerializeField]
    private Transform   legLeft;
    [SerializeField]
    private Transform   legRight;
    [SerializeField, Range(0.0f, 1.0f)]
    private float       _frustration;

    struct Rest
    {
        public Vector3      position;
        public Quaternion   rotation;
    }

    Rest    rootRest, torsoRest, headRest, armLeftRest, armRightRest, legLeftRest, legRightRest;
    float   phase;

    public float frustration
    {
        get => _frustration;
        set => _frustration = Mathf.Clamp01(value);
    }

    void Awake()
    {
        if (root == null) root = FindPart("root");
        if (torso == null) torso = FindPart("torso");
        if (head == null) head = FindPart("head");
        if (armLeft == null) armLeft = FindPart("arm-left");
        if (armRight == null) armRight = FindPart("arm-right");
        if (legLeft == null) legLeft = FindPart("leg-left");
        if (legRight == null) legRight = FindPart("leg-right");

        rootRest = GetRest(root);
        torsoRest = GetRest(torso);
        headRest = GetRest(head);
        armLeftRest = GetRest(armLeft);
        armRightRest = GetRest(armRight);
        legLeftRest = GetRest(legLeft);
        legRightRest = GetRest(legRight);
    }

    void LateUpdate()
    {
        float f = _frustration;
        float calm = 1.0f - Mathf.SmoothStep(0.0f, 1.0f, f / 0.45f);
        float impatient = Mathf.SmoothStep(0.0f, 1.0f, (f - 0.2f) / 0.35f) * (1.0f - Mathf.SmoothStep(0.0f, 1.0f, (f - 0.75f) / 0.2f));
        float furious = Mathf.SmoothStep(0.0f, 1.0f, (f - 0.7f) / 0.3f);

        // Integrated, so changing the tempo doesn't make the motion jump
        phase += Time.deltaTime * Mathf.Lerp(0.8f, 2.6f, f);

        // Hopping when furious
        Pose(root, rootRest, new Vector3(0.0f, Mathf.Abs(Wave(1.0f)) * 0.12f * furious, 0.0f), Vector3.zero);

        // Breathing, leaning in
        Pose(torso, torsoRest, new Vector3(0.0f, Wave(0.5f) * 0.015f, 0.0f),
             new Vector3(6.0f * impatient + 12.0f * furious, 0.0f, 0.0f));

        // Nodding when calm, shaking his head when impatient (and faster, looking up, when furious)
        Pose(head, headRest, Vector3.zero,
             new Vector3(Wave(0.5f) * 4.0f * calm - 12.0f * furious,
                         Wave(1.0f) * 15.0f * impatient + Wave(3.0f) * 25.0f * furious,
                         0.0f));

        // Left arm: hanging, then held out ("come on!"), then waving over his head
        float leftOut = Mathf.Lerp(4.0f + Wave(0.5f) * 2.0f * calm + 30.0f * impatient, 140.0f + Wave(2.0f) * 25.0f, furious);
        Pose(armLeft, armLeftRest, Vector3.zero, new Vector3(0.0f, 0.0f, -leftOut));

        // Right arm: hanging, then gesturing forward, then waving over his head
        float rightForward = (60.0f + Wave(2.0f) * 15.0f) * impatient * (1.0f - furious);
        float rightOut = Mathf.Lerp(4.0f + Wave(0.5f, 0.25f) * 2.0f * calm, 140.0f + Wave(2.0f, 0.25f) * 25.0f, furious);
        Pose(armRight, armRightRest, Vector3.zero, new Vector3(-rightForward, 0.0f, rightOut));

        // Tapping the right foot when impatient, stomping when furious
        float tap = Mathf.Max(0.0f, Wave(2.0f)) * 15.0f * impatient;
        float stomp = Wave(1.0f) * 12.0f * furious;
        Pose(legLeft, legLeftRest, Vector3.zero, new Vector3(stomp, 0.0f, 0.0f));
        Pose(legRight, legRightRest, Vector3.zero, new Vector3(-tap - stomp, 0.0f, 0.0f));
    }

    // Sine at frequency cycles per beat of the (frustration-driven) tempo
    float Wave(float frequency, float offset = 0.0f)
    {
        return Mathf.Sin((phase * frequency + offset) * Mathf.PI * 2.0f);
    }

    static void Pose(Transform part, Rest rest, Vector3 offset, Vector3 euler)
    {
        if (part == null) return;
        part.localPosition = rest.position + offset;
        part.localRotation = rest.rotation * Quaternion.Euler(euler);
    }

    static Rest GetRest(Transform part)
    {
        return (part != null) ? (new Rest { position = part.localPosition, rotation = part.localRotation }) : (new Rest());
    }

    Transform FindPart(string partName)
    {
        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name == partName) return t;
        }
        return null;
    }
}
