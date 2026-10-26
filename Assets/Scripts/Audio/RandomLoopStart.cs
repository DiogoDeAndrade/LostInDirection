using UnityEngine;

// Starts its AudioSource's loop at a random point of the clip, so several of the same (the fire baskets) aren't in
// step. The source shouldn't play on awake: this starts it.
[RequireComponent(typeof(AudioSource))]
public class RandomLoopStart : MonoBehaviour
{
    void Start()
    {
        var source = GetComponent<AudioSource>();
        if (source.clip == null) return;

        source.time = Random.Range(0.0f, source.clip.length);
        source.Play();
    }
}
