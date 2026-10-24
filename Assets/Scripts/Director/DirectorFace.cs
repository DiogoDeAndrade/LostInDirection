using UnityEngine;

// The director's mood on his face. His texture is replaced by a runtime copy of the base texture, and showing a face
// copies that face's sprite over the head area (the top-left of the texture, where the head is laid out). Both
// textures need Read/Write enabled.
public class DirectorFace : MonoBehaviour
{
    [SerializeField, Tooltip("The director's texture, as painted")]
    private Texture2D   baseTexture;
    [SerializeField, Tooltip("Faces from the best mood to the worst, each covering the whole head area")]
    private Sprite[]    faces;
    [SerializeField, Tooltip("Where the faces go in the base texture: their top-left corner, in pixels from the top-left")]
    private Vector2Int  faceTopLeft = Vector2Int.zero;

    Texture2D   texture;
    Material    material;
    int         face = -1;

    public int faceCount => (faces != null) ? (faces.Length) : (0);

    void Awake()
    {
        texture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, true);
        texture.name = "DirectorFace";
        texture.SetPixels(baseTexture.GetPixels());
        texture.Apply(true);

        // One material for every part, with the runtime texture
        var renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;
        material = new Material(renderers[0].sharedMaterial);
        material.name = "Director (runtime)";
        material.SetTexture("_BaseMap", texture);
        foreach (var r in renderers) r.sharedMaterial = material;
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
        if (texture != null) Destroy(texture);
    }

    // Shows face index (0 = best mood); nothing to do if it's already showing
    public void SetFace(int index)
    {
        if (faceCount == 0) return;

        index = Mathf.Clamp(index, 0, faces.Length - 1);
        if ((index == face) || (faces[index] == null)) return;
        face = index;

        var sprite = faces[index];
        var rect = sprite.textureRect;
        int width = (int)rect.width;
        int height = (int)rect.height;
        var pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, width, height);

        // Textures count rows from the bottom
        texture.SetPixels(faceTopLeft.x, texture.height - faceTopLeft.y - height, width, height, pixels);
        texture.Apply(true);
    }
}
