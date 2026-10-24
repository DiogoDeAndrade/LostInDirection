using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;

// The director's speech balloon, next to his portrait in the HUD. Say shows lines one after another, each typed out
// (with a voice blip every few letters, when there's a voice) and then held for a moment; the skip button (the
// actor's Interact) goes straight to the next line, or ends the speech on the last one. While that runs the
// director is talking (isTalking): the actor can't move or interact, and the DirectorDriver doesn't change
// frustration or mood. The director gestures while a line is being typed.
public class DirectorSpeech : MonoBehaviour
{
    [SerializeField, Tooltip("Shown while he talks")]
    private GameObject      balloon;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField, Min(1.0f)]
    private float           charactersPerSecond = 30.0f;
    [SerializeField, Min(0.0f), Tooltip("Seconds a line stays up once it's all typed")]
    private float           holdTime = 1.0f;
    [SerializeField, Min(0.0f), Tooltip("Added to the hold time per character, so longer lines stay longer")]
    private float           holdPerCharacter = 0.03f;
    [SerializeField, Tooltip("Blip played while typing (optional)")]
    private SoundDef        voice;
    [SerializeField, Min(1)]
    private int             charactersPerBlip = 3;
    [SerializeField, ShowIf(nameof(needNewInputSystem))]
    private PlayerInput     playerInput;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton, Tooltip("Skips to the next line")]
    private UC.InputControl skipInput;

    public static DirectorSpeech instance { get; private set; }

    public static bool isTalking => (instance != null) && (instance.talking);

    public bool needNewInputSystem => (skipInput != null) && (skipInput.type == UC.InputControl.InputType.NewInput);

    bool    talking;
    int     lineFrame;

    void Awake()
    {
        instance = this;
        if (balloon != null) balloon.SetActive(false);
    }

    void Start()
    {
        if (skipInput != null) skipInput.playerInput = playerInput;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    // Says the lines (what he means; translated to gibberish unless translate is off), finishing after the last one
    public IEnumerator Say(IReadOnlyList<string> lines, bool translate)
    {
        if ((lines == null) || (lines.Count == 0)) yield break;

        talking = true;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            balloon.SetActive(true);
            text.text = (translate) ? (Gibberish.Translate(line)) : (line);
            text.maxVisibleCharacters = 0;
            text.ForceMeshUpdate();
            int total = text.textInfo.characterCount;
            lineFrame = Time.frameCount;

            SetDirectorTalking(true);
            float visible = 0.0f;
            int lastBlip = 0;
            bool skipped = false;
            while (visible < total)
            {
                if (SkipPressed())
                {
                    skipped = true;
                    break;
                }
                visible += charactersPerSecond * Time.deltaTime;
                int shown = Mathf.Min(total, Mathf.FloorToInt(visible));
                text.maxVisibleCharacters = shown;
                if ((voice != null) && (shown - lastBlip >= charactersPerBlip))
                {
                    voice.Play();
                    lastBlip = shown;
                }
                yield return null;
            }
            SetDirectorTalking(false);

            float hold = (skipped) ? (0.0f) : (holdTime + holdPerCharacter * total);
            while ((hold > 0.0f) && (!SkipPressed()))
            {
                hold -= Time.deltaTime;
                yield return null;
            }
        }

        balloon.SetActive(false);
        talking = false;
    }

    // Not on the frame a line comes up: the press that skipped the last line (or the Interact that started the
    // speech) would skip this one too. Say runs after the actor's Update, so a press that ends the speech has already
    // been seen (and ignored) by the locked actor and doesn't interact with anything.
    bool SkipPressed()
    {
        return (skipInput != null) && (Time.frameCount != lineFrame) && (skipInput.IsDown());
    }

    static void SetDirectorTalking(bool value)
    {
        if (Director.instance != null) Director.instance.talking = value;
    }
}
