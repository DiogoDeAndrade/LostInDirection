using System.Collections.Generic;
using UnityEngine;

// Blood on the floor: a flat quad with one of the four splats in its material's 2x2 atlas, picked at random, turned
// at random. It grows to its size over its grow time (a splash: at once; a pool under a body: slowly), stays lifetime
// seconds, then fades out over fadeTime and is gone. Never more than maxDecals at once: past that the oldest fade early.
// Spawn() finds the floor under a point (skipping orcs and the actor) and lays one there.
public class BloodDecal : MonoBehaviour
{
    [SerializeField, Min(0.0f)]
    private float   lifetime = 25.0f;
    [SerializeField, Min(0.01f)]
    private float   fadeTime = 2.0f;
    [SerializeField, Min(1)]
    private int     maxDecals = 60;

    static List<BloodDecal> active = new List<BloodDecal>();
    static int              layerCounter;

    Renderer                quad;
    MaterialPropertyBlock   block;
    Color                   baseColor;
    float                   size;
    float                   growTime;
    float                   age;
    float                   fadeStart;

    public static BloodDecal Spawn(BloodDecal prefab, Vector3 point, float size, float growTime)
    {
        if (prefab == null) return null;

        var hits = Physics.RaycastAll(point + Vector3.up * 0.5f, Vector3.down, 2.0f, ~0, QueryTriggerInteraction.Ignore);
        RaycastHit? floor = null;
        foreach (var hit in hits)
        {
            if ((hit.collider.GetComponentInParent<Orc>() != null) || (hit.collider.GetComponentInParent<Player>() != null)) continue;
            if ((floor == null) || (hit.distance < floor.Value.distance)) floor = hit;
        }
        if (floor == null) return null;

        // A hair above the floor, each a little higher than the last, so they don't fight each other or the floor
        layerCounter = (layerCounter + 1) % 16;
        var normal = floor.Value.normal;
        var position = floor.Value.point + normal * (0.004f + layerCounter * 0.0006f);
        var rotation = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
        var decal = Instantiate(prefab, position, rotation);
        decal.size = size;
        decal.growTime = growTime;
        return decal;
    }

    void Awake()
    {
        quad = GetComponentInChildren<Renderer>();
        block = new MaterialPropertyBlock();
        baseColor = quad.sharedMaterial.GetColor("_BaseColor");
        int tile = Random.Range(0, 4);
        block.SetVector("_BaseMap_ST", new Vector4(0.5f, 0.5f, (tile % 2) * 0.5f, (tile / 2) * 0.5f));
        fadeStart = lifetime;
        transform.localScale = Vector3.zero;
    }

    void OnEnable()
    {
        active.Add(this);
        if (active.Count > maxDecals)
        {
            foreach (var decal in active)
            {
                if (decal.fadeStart > decal.age) { decal.FadeNow(); break; }
            }
        }
    }

    void OnDisable()
    {
        active.Remove(this);
    }

    void FadeNow()
    {
        fadeStart = Mathf.Min(fadeStart, age);
    }

    void Update()
    {
        age += Time.deltaTime;

        float grow = (growTime > 0.0f) ? (Mathf.Clamp01(age / growTime)) : (1.0f);
        grow = 1.0f - (1.0f - grow) * (1.0f - grow);
        transform.localScale = Vector3.one * (size * grow);

        float alpha = 1.0f - Mathf.Clamp01((age - fadeStart) / fadeTime);
        var color = baseColor;
        color.a *= alpha;
        block.SetColor("_BaseColor", color);
        quad.SetPropertyBlock(block);

        if (alpha <= 0.0f) Destroy(gameObject);
    }
}
