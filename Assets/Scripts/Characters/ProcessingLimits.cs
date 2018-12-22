[System.Serializable]
public class ProcessingLimits {
    public float delayAfterAttacking = 1;

    internal bool ready;
    /// <summary>
    /// Temporary stun wait, that get's reset after 1 use.
    /// </summary>
    public float waitStun;
}
