using System.Collections;
using System.Collections.Generic;
using TMPro;
using UC;
using UnityEngine;

// The director's speech balloon, next to his portrait in the HUD. Say shows lines one after another, each typed out
// (with a voice blip every few letters, when there's a voice) and then held for a moment. While that runs the
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

    public static DirectorSpeech instance { get; private set; }

    public static bool isTalking => (instance != null) && (instance.talking);

    bool    talking;

    void Awake()
    {
        instance = this;
        if (balloon != null) balloon.SetActive(false);
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

            SetDirectorTalking(true);
            float visible = 0.0f;
            int lastBlip = 0;
            while (visible < total)
            {
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

            yield return new WaitForSeconds(holdTime + holdPerCharacter * total);
        }

        balloon.SetActive(false);
        talking = false;
    }

    static void SetDirectorTalking(bool value)
    {
        if (Director.instance != null) Director.instance.talking = value;
    }
}
