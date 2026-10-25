using UnityEngine;

// Turns slowly and bobs where it is (a floating artifact)
public class Spinner : MonoBehaviour
{
    [SerializeField, Tooltip("Degrees per second")]
    private float   spinSpeed = 45.0f;
    [SerializeField]
    private float   bobHeight = 0.05f;
    [SerializeField]
    private float   bobSpeed = 1.5f;

    Vector3 basePosition;

    void Start()
    {
        basePosition = transform.localPosition;
    }

    void Update()
    {
        transform.Rotate(0.0f, spinSpeed * Time.deltaTime, 0.0f, Space.World);
        transform.localPosition = basePosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed * Mathf.PI * 2.0f) * bobHeight);
    }
}
