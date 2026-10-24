// Whether gameplay is on hold: a cutscene, the director talking, the slate clapping, or the take cut. While it is,
// the actor has no control, zombies stand still and the DirectorDriver changes nothing.
public static class Gameplay
{
    public static bool isHeld => (Cutscene.isActive) || (DirectorSpeech.isTalking) || (Slate.isPlaying) || (GameManager.isCut);
}
