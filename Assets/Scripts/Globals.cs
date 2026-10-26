using UC;
using UnityEngine;

// The game's UnityCommon globals (Assets/Resources/Globals.asset): UI sounds and the like, which UnityCommon systems
// such as the menus look up, and the game's own sounds that aren't tied to a scene: the actor performing a verb,
// cycling an object's verbs, an item going into the inventory, getting hurt (a trap, an orc)
[CreateAssetMenu(fileName = "Globals", menuName = "Lost in Direction/Global Configuration")]
public class Globals : GlobalsBase
{
    [SerializeField, Tooltip("The actor performing a verb (before the verb's own sound)")]
    private SoundDef    _interactSnd;
    [SerializeField]
    private SoundDef    _verbCycleSnd;
    [SerializeField]
    private SoundDef    _pickupSnd;
    [SerializeField, Tooltip("The actor getting hurt (with the red flash and \"Ouch!\")")]
    private SoundDef    _hurtSnd;

    public static Globals instance => GetInstanceBase<Globals>();

    public static SoundDef interactSnd => (instance != null) ? (instance._interactSnd) : (null);
    public static SoundDef verbCycleSnd => (instance != null) ? (instance._verbCycleSnd) : (null);
    public static SoundDef pickupSnd => (instance != null) ? (instance._pickupSnd) : (null);
    public static SoundDef hurtSnd => (instance != null) ? (instance._hurtSnd) : (null);
}
