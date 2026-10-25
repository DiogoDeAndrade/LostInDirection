using UC;
using UnityEngine;

// The game's UnityCommon globals (Assets/Resources/Globals.asset): UI sounds and the like, which UnityCommon systems
// such as the menus look up
[CreateAssetMenu(fileName = "Globals", menuName = "Lost in Direction/Global Configuration")]
public class Globals : GlobalsBase
{
    public static Globals instance => GetInstanceBase<Globals>();
}
