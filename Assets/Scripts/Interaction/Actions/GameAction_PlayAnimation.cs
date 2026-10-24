using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Plays an animation clip once on a character: through its CharacterAnimation, so idle/walk resumes afterwards,
// or straight on its PlayableHandler when it has none. With "wait", the next action starts when the clip ends.
[Serializable]
[GameActionName("Animation/Play")]
public class GameAction_PlayAnimation : GameAction
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Actor);
    [SerializeField]
    private AnimationClip   clip;
    [SerializeField, Min(0), Tooltip("Cross-fade time into the clip, in seconds")]
    private float           transitionTime = 0.1f;

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        if ((obj == null) || (clip == null))
        {
            Debug.LogWarning($"Play Animation: no {((obj == null) ? ($"object ({target})") : ("clip"))}");
            yield break;
        }

        var characterAnimation = obj.GetComponentInChildren<CharacterAnimation>();
        if (characterAnimation != null)
        {
            characterAnimation.PlayOneShot(clip, transitionTime);
        }
        else
        {
            var handler = obj.GetComponentInChildren<PlayableHandler>();
            if (handler == null)
            {
                Debug.LogWarning($"Play Animation: {obj.name} has no CharacterAnimation or PlayableHandler");
                yield break;
            }
            handler.Play(clip, false, transitionTime);
        }

        yield return new WaitForSeconds(clip.length);
    }
}
