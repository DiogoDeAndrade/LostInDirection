using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Label next to the object the actor can interact with: its name, its verbs (the selected one highlighted) and
// the buttons that cycle and perform them. Lives on a screen-space canvas and follows the object on screen;
// hidden while there's no target.
public class InteractionPrompt : MonoBehaviour
{
    [SerializeField, Tooltip("What gets moved, shown and hidden; its pivot is the point placed next to the object")]
    private RectTransform   panel;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField, Tooltip("Copied once per verb, next to itself; kept disabled")]
    private TextMeshProUGUI verbTemplate;
    [SerializeField]
    private TextMeshProUGUI hintText;
    [SerializeField]
    private Color           verbColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField]
    private Color           selectedVerbColor = new Color(1.0f, 0.85f, 0.3f);
    [SerializeField, Tooltip("Put before the selected verb")]
    private string          selectedPrefix = "» ";
    [SerializeField, Tooltip("Put before the other verbs (spaces, to line them up with the selected one)")]
    private string          unselectedPrefix = "   ";
    [SerializeField, Tooltip("Offset from the object's prompt anchor on screen, in canvas units")]
    private Vector2         offset = new Vector2(40.0f, 0.0f);
    [SerializeField]
    private string          gamepadHint = "(Y) next   (X) do it";
    [SerializeField]
    private string          keyboardHint = "[Q] next   [E] do it";

    List<TextMeshProUGUI>   verbTexts = new List<TextMeshProUGUI>();
    Interactable            target;
    int                     shownVerb = -1;
    bool                    shownGamepad;
    Canvas                  canvas;
    RectTransform           canvasRect;
    Camera                  mainCamera;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>().rootCanvas;
        canvasRect = canvas.GetComponent<RectTransform>();
        verbTemplate.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
    }

    // Called every frame by the actor; only rebuilds the text when something changed
    public void Show(Interactable newTarget, int verbIndex, bool gamepad)
    {
        if ((newTarget == target) && (verbIndex == shownVerb) && (gamepad == shownGamepad)) return;

        target = newTarget;
        shownVerb = verbIndex;
        shownGamepad = gamepad;

        panel.gameObject.SetActive(target != null);
        if (target == null) return;

        nameText.text = target.objectName;

        var verbs = target.verbs;
        while (verbTexts.Count < verbs.Count)
        {
            var text = Instantiate(verbTemplate, verbTemplate.transform.parent);
            verbTexts.Add(text);
        }
        for (int i = 0; i < verbTexts.Count; i++)
        {
            bool used = (i < verbs.Count);
            verbTexts[i].gameObject.SetActive(used);
            if (!used) continue;

            bool selected = (i == verbIndex);
            verbTexts[i].text = ((selected) ? (selectedPrefix) : (unselectedPrefix)) + verbs[i];
            verbTexts[i].color = (selected) ? (selectedVerbColor) : (verbColor);
        }

        hintText.text = (gamepad) ? (gamepadHint) : (keyboardHint);

        // The size changed with the text, and Place needs it to keep the panel on screen
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        Place();
    }

    void LateUpdate()
    {
        if (target != null) Place();
    }

    // Puts the panel next to the target on screen, kept inside the canvas
    void Place()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.promptPosition);
        Camera uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? (null) : (canvas.worldCamera);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out Vector2 local)) return;

        Vector2 position = local + offset;

        Rect canvasArea = canvasRect.rect;
        Vector2 size = panel.rect.size;
        Vector2 pivot = panel.pivot;
        position.x = Mathf.Clamp(position.x, canvasArea.xMin + size.x * pivot.x, canvasArea.xMax - size.x * (1.0f - pivot.x));
        position.y = Mathf.Clamp(position.y, canvasArea.yMin + size.y * pivot.y, canvasArea.yMax - size.y * (1.0f - pivot.y));

        panel.anchoredPosition = position;
    }
}
