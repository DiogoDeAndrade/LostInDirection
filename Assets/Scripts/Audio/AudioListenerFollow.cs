using UC;
using UnityEngine;

// The audio listener, on its own object, kept at the actor's head (the first object with targetTag) and turned like
// the camera drawing the screen: 3D sounds are as loud as they are near the actor, not near the camera high above
// him, and their left and right match the screen's (the cutscene camera's, during a cutscene)
[RequireComponent(typeof(AudioListener))]
public class AudioListenerFollow : MonoBehaviour
{
    [SerializeField, Tooltip("Who it listens from")]
    private Hypertag    targetTag;
    [SerializeField]
    private Vector3     offset = new Vector3(0.0f, 1.0f, 0.0f);

    Transform target;

    void LateUpdate()
    {
        if (target == null)
        {
            target = (targetTag != null) ? (Hypertag.FindFirstObjectWithHypertag<Transform>(targetTag)) : (null);
            if (target == null) return;
        }

        transform.position = target.position + offset;
        var view = ViewCamera();
        if (view != null) transform.rotation = Quaternion.Euler(0.0f, view.transform.eulerAngles.y, 0.0f);
    }

    // The enabled camera drawing to the screen with the highest depth (the director's draws to a texture)
    static Camera ViewCamera()
    {
        Camera best = null;
        foreach (var c in Camera.allCameras)
        {
            if (c.targetTexture != null) continue;
            if ((best == null) || (c.depth > best.depth)) best = c;
        }
        return best;
    }
}
