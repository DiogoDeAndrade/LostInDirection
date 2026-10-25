using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Plays a song through UnityCommon's SoundManager, crossfading from whatever is playing (the movies start theirs on
// "Action!"). No clip fades the music out.
[Serializable]
[GameActionName("Sound/Play Music")]
public class GameAction_PlayMusic : GameAction
{
    [SerializeField]
    private AudioClip   clip;
    [SerializeField, Range(0.0f, 1.0f)]
    private float       volume = 1.0f;
    [SerializeField, Min(0.0f)]
    private float       crossfadeTime = 1.0f;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        SoundManager.PlayMusic(clip, volume, 1.0f, crossfadeTime);
        yield break;
    }
}
