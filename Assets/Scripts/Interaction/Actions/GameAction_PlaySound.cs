using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Plays a sound (a SoundDef), delay seconds in (to land on an animation's hit), repeat times interval seconds apart
// (knock, knock, knock). With a duration, each is cut after that long, fading out over fadeOut, so a long recording
// (digging) stops when what it goes with does. A 3D SoundDef (its Default 3D flag) is heard from the object, where it
// was when the action started. With "wait", the next action starts once the last one has started (or, with a
// duration, once it's been cut).
[Serializable]
[GameActionName("Sound/Play")]
public class GameAction_PlaySound : GameAction
{
    [SerializeField]
    private SoundDef        sound;
    [SerializeField, Tooltip("Where a 3D sound is heard from")]
    private ObjectTarget    from = new ObjectTarget(ObjectTarget.Mode.Interactable);
    [SerializeField, Min(0.0f), Tooltip("Seconds before it starts")]
    private float           delay = 0.0f;
    [SerializeField, Min(1)]
    private int             repeat = 1;
    [SerializeField, Min(0.0f), Tooltip("Seconds between repeats")]
    private float           interval = 0.25f;
    [SerializeField, Min(0.0f), Tooltip("Seconds before it's cut (0: it plays to its end)")]
    private float           duration = 0.0f;
    [SerializeField, Min(0.0f), Tooltip("Seconds it fades out over when it's cut")]
    private float           fadeOut = 0.15f;

    public override IEnumerator Execute(ActionContext context)
    {
        if (sound == null)
        {
            Debug.LogWarning("Play Sound: no sound set");
            yield break;
        }

        var obj = from.GetGameObject(context);
        Vector3 position = (obj != null) ? (obj.transform.position) : (Vector3.zero);

        if (delay > 0.0f) yield return new WaitForSeconds(delay);
        for (int i = 0; i < repeat; i++)
        {
            if (i > 0) yield return new WaitForSeconds(interval);
            var source = sound.Play(1.0f, 1.0f, -float.MaxValue, position);
            if ((duration <= 0.0f) || (source == null)) continue;

            if ((i < repeat - 1) && (context.runner != null)) context.runner.StartCoroutine(CutCR(source));
            else yield return CutCR(source);
        }
    }

    // The SoundManager's sources are pooled: once this one stops playing this clip it's someone else's, so leave it
    IEnumerator CutCR(AudioSource source)
    {
        var clip = source.clip;
        yield return new WaitForSeconds(duration);

        float start = source.volume;
        for (float t = 0.0f; t < fadeOut; t += Time.deltaTime)
        {
            if ((source == null) || (source.clip != clip) || (!source.isPlaying)) yield break;
            source.volume = start * (1.0f - t / fadeOut);
            yield return null;
        }
        if ((source != null) && (source.clip == clip)) source.Stop();
    }
}
