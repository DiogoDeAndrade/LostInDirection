using System.Collections.Generic;
using UC;
using UC.Interaction;
using UnityEngine;

// Something on the set the actor can interact with. What can be done with it are the Verbs on this object and its
// children, found on Start, in hierarchy order (which is the order the prompt cycles through them). When the actor
// is close and facing it, and at least one verb is available, it gets outlined and the interaction prompt shows
// its name and available verbs.
[RequireComponent(typeof(ScreenSpaceOutline))]
public class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip("Name shown on the prompt; the object's name if left empty")]
    private string          displayName;
    [SerializeField, Tooltip("Where the prompt is anchored, as an offset from this object's position")]
    private Vector3         promptOffset = new Vector3(0.0f, 0.5f, 0.0f);

    static List<Interactable> _all = new List<Interactable>();

    // Every enabled interactable in the scene
    public static IReadOnlyList<Interactable> all => _all;

    ScreenSpaceOutline  outline;
    Collider            mainCollider;
    List<Verb>          verbs = new List<Verb>();

    public string objectName => (string.IsNullOrEmpty(displayName)) ? (name) : (displayName);
    public Vector3 promptPosition => transform.position + promptOffset;

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

    void Start()
    {
        GetComponentsInChildren(true, verbs);
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

    // Fills result with the verbs that can be performed right now, in order (verbs destroyed by an action are skipped)
    public void GetAvailableVerbs(ActionContext context, List<Verb> result)
    {
        result.Clear();
        if (!isActiveAndEnabled) return;

        context.targetGameObject = gameObject;
        foreach (var verb in verbs)
        {
            if ((verb != null) && (verb.IsAvailable(context))) result.Add(verb);
        }
    }

    // Point of the object closest to a position: on its collider, or its pivot when it has none that supports it
    public Vector3 ClosestPoint(Vector3 position)
    {
        if ((mainCollider == null) || (!mainCollider.enabled)) return transform.position;
        if ((mainCollider is MeshCollider meshCollider) && (!meshCollider.convex)) return mainCollider.bounds.ClosestPoint(position);

        return mainCollider.ClosestPoint(position);
    }

    public void Interact(GameObject actor, ActionRunner runner, Verb verb)
    {
        if (verb == null) return;

        Debug.Log($"{actor.name}: {verb.displayName} {objectName}");

        var context = new ActionContext
        {
            triggerGameObject = actor,
            targetGameObject = gameObject,
            runner = runner
        };
        verb.Perform(context);
    }
}
