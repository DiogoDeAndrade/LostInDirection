using UnityEngine;
using UnityEngine.UI;

// A full-screen colour flash (the HUD's "HitFlash", red, under the rest of the HUD): Flash() brings its image up to
// peakAlpha almost at once and lets it fade out. For when the actor gets hurt.
[RequireComponent(typeof(Image))]
public class ScreenFlash : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float   peakAlpha = 0.4f;
    [SerializeField, Min(0.01f)]
    private float   riseTime = 0.05f;
    [SerializeField, Min(0.01f)]
    private float   fadeTime = 0.6f;

    public static ScreenFlash instance { get; private set; }

    Image   image;
    float   elapsed = float.MaxValue;

    void Awake()
    {
        instance = this;
        image = GetComponent<Image>();
        SetAlpha(0.0f);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static void Flash()
    {
        if (instance != null) instance.elapsed = 0.0f;
    }

    void Update()
    {
        if (elapsed > riseTime + fadeTime) return;

        elapsed += Time.deltaTime;
        float alpha = (elapsed < riseTime) ? (elapsed / riseTime) : (1.0f - Mathf.Clamp01((elapsed - riseTime) / fadeTime));
        SetAlpha(alpha * peakAlpha);
    }

    void SetAlpha(float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
