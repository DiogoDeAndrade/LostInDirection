using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Plays an animation clip on an object without an Animator (a prop's own clips: a gate opening, a chest's lid), by
// sampling it frame by frame over its length; it ends on the clip's last pose. With "wait", the next action starts
// when it's done.
[Serializable]
[GameActionName("Animation/Play Clip")]
public class GameAction_PlayClip : GameAction
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField]
    private AnimationClip   clip;
    [SerializeField, Min(0.01f)]
    private float           speed = 1.0f;

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        if ((obj == null) || (clip == null))
        {
            Debug.LogWarning($"Play Clip: no object ({target}) or clip");
            yield break;
        }

        for (float t = 0.0f; t < clip.length; t += Time.deltaTime * speed)
        {
            if (obj == null) yield break;
            clip.SampleAnimation(obj, t);
            yield return null;
        }
        if (obj != null) clip.SampleAnimation(obj, clip.length);
    }
}
