using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The takes left (GameManager), one slate icon each, in a row (a layout group on this object; the HUD's top right).
// When a take is lost, the first icon swells and fades out, so the others don't move.
public class TakesDisplay : MonoBehaviour
{
    [SerializeField]
    private Image   iconPrefab;
    [SerializeField, Min(0.01f), Tooltip("Seconds a lost take's icon takes to go")]
    private float   loseTime = 0.6f;
    [SerializeField, Min(1.0f), Tooltip("How much it swells while it goes")]
    private float   loseScale = 1.5f;

    List<Image> icons = new List<Image>();

    void Update()
    {
        var gameManager = GameManager.instance;
        int left = (gameManager != null) ? (gameManager.takesLeft) : (0);

        while (icons.Count < left)
        {
            icons.Add(Instantiate(iconPrefab, transform));
        }
        while (icons.Count > left)
        {
            var icon = icons[0];
            icons.RemoveAt(0);
            StartCoroutine(LoseCR(icon));
        }
    }

    IEnumerator LoseCR(Image icon)
    {
        var rect = icon.rectTransform;
        var color = icon.color;
        for (float t = 0.0f; t < loseTime; t += Time.deltaTime)
        {
            float k = t / loseTime;
            rect.localScale = Vector3.one * Mathf.Lerp(1.0f, loseScale, k);
            icon.color = new Color(color.r, color.g, color.b, color.a * (1.0f - k));
            yield return null;
        }
        Destroy(icon.gameObject);
    }
}
