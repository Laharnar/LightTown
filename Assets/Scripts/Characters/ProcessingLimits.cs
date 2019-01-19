using System.Collections;
using UnityEngine;

[System.Serializable]
public class ProcessingLimits {
    public float delayAfterAttacking = 1;

    internal bool ready;
    /// <summary>
    /// Temporary stun wait, that get's reset after 1 use.
    /// </summary>
    public float waitStun;

    /// <summary>
    /// handles delays between attacks
    /// </summary>
    /// <returns></returns>
    public IEnumerator CharacterLimits() {
        while (true) {
            // delay after attacking is set in inspector. don't remove this code.
            if (delayAfterAttacking > 0 && !ready) {
                yield return new WaitForSeconds(delayAfterAttacking);
                ready = true;
                Debug.Log("ready to attack.");
            } else yield return null;
        }
    }
}
