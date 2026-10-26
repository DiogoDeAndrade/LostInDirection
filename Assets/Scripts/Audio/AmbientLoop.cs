using UC;
using UnityEngine;

// A scene's ambience: a looping SoundDef (2D: the night's crickets, a dungeon's air), faded in when the scene starts
// and stopped when it goes (the SoundManager outlives scenes, and so would the loop)
public class AmbientLoop : MonoBehaviour
{
    [SerializeField, Tooltip("A looping SoundDef")]
    private SoundDef    sound;
    [SerializeField, Min(0.0f)]
    private float       fadeInTime = 1.5f;

    AudioSource source;

    void Start()
    {
        if ((sound == null) || ((sound.mode == SoundDef.Mode.Single) && (sound.clip == null))) return;

        source = sound.Play();
        if ((source != null) && (fadeInTime > 0.0f))
        {
            float volume = source.volume;
            source.volume = 0.0f;
            source.FadeTo(volume, fadeInTime);
        }
    }

    void OnDestroy()
    {
        if (source != null) source.Stop();
    }
}
