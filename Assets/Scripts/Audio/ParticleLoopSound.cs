using UnityEngine;

// A looping sound that goes with a particle system (the girl's spell): it fades in when the system starts emitting
// and out when it stops, so whatever starts and stops the particles (the Play and Stop Particle System actions) does
// the sound too. On an AudioSource set to loop and not to play on awake; its volume is the one it fades to.
[RequireComponent(typeof(AudioSource))]
public class ParticleLoopSound : MonoBehaviour
{
    [SerializeField, Tooltip("This object's if empty")]
    private ParticleSystem  particles;
    [SerializeField, Min(0.0f)]
    private float           fadeInTime = 0.6f;
    [SerializeField, Min(0.0f)]
    private float           fadeOutTime = 0.4f;

    AudioSource source;
    float       volume;
    float       level;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (particles == null) particles = GetComponent<ParticleSystem>();
        volume = source.volume;
        source.volume = 0.0f;
    }

    void Update()
    {
        bool on = (particles != null) && (particles.isEmitting);
        float time = (on) ? (fadeInTime) : (fadeOutTime);
        level = (time > 0.0f) ? (Mathf.MoveTowards(level, (on) ? (1.0f) : (0.0f), Time.deltaTime / time)) : ((on) ? (1.0f) : (0.0f));

        if ((level > 0.0f) && (!source.isPlaying)) source.Play();
        source.volume = volume * level;
        if ((level <= 0.0f) && (source.isPlaying)) source.Stop();
    }
}
