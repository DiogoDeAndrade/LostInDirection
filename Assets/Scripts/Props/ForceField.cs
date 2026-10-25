using System.Collections;
using UnityEngine;

// A force field (a see-through glowing sphere with a solid collider, and a light): it pulses gently. Lower() fades it
// out while it swells a little, with the dissolve particles (sparks off its surface that drift down after it's gone),
// then switches it off, collider and all.
public class ForceField : MonoBehaviour
{
    [SerializeField, Min(0.0f)]
    private float   pulseSpeed = 1.5f;
    [SerializeField, Range(0.0f, 0.5f), Tooltip("How much the alpha and the size breathe")]
    private float   pulseAmount = 0.15f;
    [SerializeField, Min(0.01f)]
    private float   lowerTime = 1.2f;
    [SerializeField, Tooltip("Played as it goes down (not a child: it outlives the field)")]
    private ParticleSystem  dissolve;

    Renderer[]  renderers;
    Light       fieldLight;
    Color[]     baseColors;
    float       baseLight;
    Vector3     baseScale;
    float       fade = 1.0f;
    bool        lowering;

    public bool isLowered { get; private set; }

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        baseColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) baseColors[i] = renderers[i].material.color;
        fieldLight = GetComponentInChildren<Light>();
        if (fieldLight != null) baseLight = fieldLight.intensity;
        baseScale = transform.localScale;
    }

    void Update()
    {
        float pulse = 1.0f + Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2.0f) * pulseAmount;
        float swell = (lowering) ? (1.0f + (1.0f - fade) * 0.3f) : (1.0f);
        transform.localScale = baseScale * (swell * (1.0f + (pulse - 1.0f) * 0.2f));
        for (int i = 0; i < renderers.Length; i++)
        {
            var c = baseColors[i];
            c.a *= pulse * fade;
            renderers[i].material.color = c;
        }
        if (fieldLight != null) fieldLight.intensity = baseLight * pulse * fade;
    }

    public IEnumerator Lower()
    {
        if ((lowering) || (isLowered)) yield break;
        lowering = true;
        if (dissolve != null) dissolve.Play(true);
        for (float t = 0.0f; t < lowerTime; t += Time.deltaTime)
        {
            fade = 1.0f - t / lowerTime;
            yield return null;
        }
        fade = 0.0f;
        isLowered = true;
        gameObject.SetActive(false);
    }
}
