using UnityEngine;

// Puts the object at one of its spots, picked at random when the scene starts (the temple's key, in one of the orc
// alcoves, somewhere else every take)
public class RandomSpot : MonoBehaviour
{
    [SerializeField]
    private Transform[]     spots;

    void Awake()
    {
        if ((spots == null) || (spots.Length == 0)) return;

        var spot = spots[Random.Range(0, spots.Length)];
        if (spot != null) transform.SetPositionAndRotation(spot.position, spot.rotation);
    }
}
