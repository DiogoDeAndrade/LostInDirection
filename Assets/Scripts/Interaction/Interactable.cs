using System.Collections.Generic;
using UC;
using UnityEngine;

// Something on the set the actor can interact with. When the actor is close and facing it, it gets outlined and
// the interaction prompt shows its name and verbs; the selected verb is performed through Interact.
[RequireComponent(typeof(ScreenSpaceOutline))]
public class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip("Name shown on the prompt; the object's name if left empty")]
    private string          displayName;
    [SerializeField, Tooltip("What the actor can do with it, in the order the prompt cycles through them")]
    private List<string>    _verbs = new List<string>() { "Use" };
    [SerializeField, Tooltip("Where the prompt is anchored, as an offset from this object's position")]
    private Vector3         promptOffset = new Vector3(0.0f, 0.5f, 0.0f);

    static List<Interactable> _all = new List<Interactable>();

    // Every enabled interactable in the scene
    public static IReadOnlyList<Interactable> all => _all;

    ScreenSpaceOutline  outline;
    Collider            mainCollider;

    public string objectName => (string.IsNullOrEmpty(displayName)) ? (name) : (displayName);
    public IReadOnlyList<string> verbs => _verbs;
    public Vector3 promptPosition => transform.position + promptOffset;
    public bool canInteract => (isActiveAndEnabled) && (_verbs.Count > 0);

    // Added in the editor: the outline only shows while the actor is targeting this
    void Reset()
    {
        var o = GetComponent<ScreenSpaceOutline>();
        if (o == null) return;

        o.outlineColor = new Color(1.0f, 0.85f, 0.3f);
        o.outlineWidth = 3.0f;
        o.enabled = false;
    }

    void Awake()
    {
        outline = GetComponent<ScreenSpaceOutline>();
        mainCollider = GetComponentInChildren<Collider>();

        SetHighlight(false);
    }

    void OnEnable()
    {
        _all.Add(this);
    }

    void OnDisable()
    {
        _all.Remove(this);
        SetHighlight(false);
    }

    public void SetHighlight(bool highlight)
    {
        if (outline) outline.enabled = highlight;
    }

    // Point of the object closest to a position: on its collider, or its pivot when it has none that supports it
    public Vector3 ClosestPoint(Vector3 position)
    {
        if ((mainCollider == null) || (!mainCollider.enabled)) return transform.position;
        if ((mainCollider is MeshCollider meshCollider) && (!meshCollider.convex)) return mainCollider.bounds.ClosestPoint(position);

        return mainCollider.ClosestPoint(position);
    }

    public void Interact(Player player, int verbIndex)
    {
        if ((verbIndex < 0) || (verbIndex >= _verbs.Count)) return;

        Debug.Log($"{player.name}: {_verbs[verbIndex]} {objectName}");
    }
}
