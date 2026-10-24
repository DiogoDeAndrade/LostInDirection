// Whether gameplay is on hold: a cutscene, the director talking, or the slate clapping. While it is, the actor has no
// control and the DirectorDriver changes nothing.
public static class Gameplay
{
    public static bool isHeld => (Cutscene.isActive) || (DirectorSpeech.isTalking) || (Slate.isPlaying);
}
