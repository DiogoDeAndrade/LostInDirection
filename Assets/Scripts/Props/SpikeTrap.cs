using UC;
using UnityEngine;

// A spike trap (Kenney's Mini Dungeon trap: its "spikes" part rises and sinks with the model's show / hide clips,
// sampled here). It cycles: down for downTime, up for upTime, starting phase seconds into the cycle so the traps
// along a corridor don't move in step. An actor standing on it while the spikes are up gets hurt once per cycle: the
// director's mood drops by hitMoodCost (two faces), the screen flashes red, "Ouch!" floats over him, and he's pushed
// off it along the corridor, out the side he's nearest to (back against his facing when he's dead centre). Nothing
// hurts while gameplay is on hold. The spikes make a sound going up and coming down, from the trap, only within
// hearingDistance of the listener (there are many traps, cycling all the time, and a 3D sound never quite fades out).
public class SpikeTrap : MonoBehaviour
{
    [SerializeField, Tooltip("The trap model (the clips are sampled on it); this object if empty")]
    private GameObject      model;
    [SerializeField]
    private AnimationClip   showClip;
    [SerializeField]
    private AnimationClip   hideClip;
    [SerializeField, Min(0.1f)]
    private float           downTime = 1.6f;
    [SerializeField, Min(0.1f)]
    private float           upTime = 1.0f;
    [SerializeField, Min(0.0f), Tooltip("Seconds into the cycle it starts at")]
    private float           phase;
    [SerializeField, Min(0.0f), Tooltip("Half the size of the area that hurts, from the centre (the model is 0.78 across)")]
    private float           halfSize = 0.39f;
    [SerializeField, Range(0.0f, 1.0f), Tooltip("A face is 1/7 of the mood")]
    private float           hitMoodCost = 2.0f / 7.0f;
    [SerializeField, Min(0.0f), Tooltip("How far from the trap's centre the actor ends up")]
    private float           pushDistance = 0.65f;
    [SerializeField]
    private SoundDef        upSound;
    [SerializeField]
    private SoundDef        downSound;
    [SerializeField, Min(0.0f), Tooltip("Heard only this close to the listener")]
    private float           hearingDistance = 6.0f;

    static AudioListener    listener;

    Player      actor;
    Rigidbody   actorBody;
    float       time;
    bool        up;
    bool        hitThisCycle;

    public bool isUp => up;

    void Start()
    {
        if (model == null) model = gameObject;
        actor = FindAnyObjectByType<Player>();
        if (actor != null) actorBody = actor.GetComponent<Rigidbody>();
        time = phase;
        Pose(false, 1.0f);
    }

    void Update()
    {
        float cycle = downTime + upTime;
        time = (time + Time.deltaTime) % cycle;

        bool wasUp = up;
        up = time >= downTime;
        if (up != wasUp)
        {
            hitThisCycle = false;
            PlayNear((up) ? (upSound) : (downSound));
        }

        // The clips play at the start of each half: rising as the up time starts, sinking as the down time starts
        if (up) Pose(true, time - downTime);
        else Pose(false, time);

        if ((up) && (!hitThisCycle) && (actor != null) && (!Gameplay.isHeld) && (IsOnIt(actor.transform.position))) Hit();
    }

    void PlayNear(SoundDef sound)
    {
        if (sound == null) return;
        if (listener == null) listener = FindAnyObjectByType<AudioListener>();
        if ((listener != null) && ((listener.transform.position - transform.position).sqrMagnitude > hearingDistance * hearingDistance)) return;
        sound.Play(1.0f, 1.0f, -float.MaxValue, transform.position);
    }

    void Pose(bool show, float t)
    {
        var clip = (show) ? (showClip) : (hideClip);
        if (clip != null) clip.SampleAnimation(model, Mathf.Min(t, clip.length));
    }

    bool IsOnIt(Vector3 position)
    {
        Vector3 local = position - transform.position;
        return (Mathf.Abs(local.x) < halfSize) && (Mathf.Abs(local.z) < halfSize);
    }

    void Hit()
    {
        hitThisCycle = true;

        if (DirectorDriver.instance != null) DirectorDriver.instance.ChangeMood(-hitMoodCost);
        Globals.hurtSnd?.Play();
        ScreenFlash.Flash();
        CombatTextManager.SpawnText(actor.gameObject, new Vector2(0.0f, 1.0f), "Ouch!", new CombatTextDef(new Color(1.0f, 0.35f, 0.25f), 1.0f) { followOwner = false });

        // Along the axis he's furthest off the centre on (the corridor's, coming in), or against his facing at the centre
        Vector3 away = actor.transform.position - transform.position;
        away.y = 0.0f;
        if (away.sqrMagnitude < 0.0001f) away = -actor.transform.forward;
        away = (Mathf.Abs(away.x) > Mathf.Abs(away.z)) ? (new Vector3(Mathf.Sign(away.x), 0.0f, 0.0f)) : (new Vector3(0.0f, 0.0f, Mathf.Sign(away.z)));

        Vector3 target = transform.position + away * pushDistance;
        target.y = actor.transform.position.y;
        if (actorBody != null)
        {
            actorBody.position = target;
            actorBody.linearVelocity = Vector3.zero;
        }
        else
        {
            actor.transform.position = target;
        }
    }
}
