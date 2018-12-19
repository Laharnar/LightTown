[System.Serializable]
public class ProcessingLimits {
    public float waitBetweenIssuingAbilities = 1;

    public float time_waitBetweenIssuingAbilities=0;
    internal bool ready;
    /// <summary>
    /// Temporary stun wait, that get's reset after 1 use.
    /// </summary>
    public float waitStun;
}
