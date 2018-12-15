using System;
using UnityEngine;

public static class CombatProcessing {
    // edit depending on game.
    public static void ProcessAction(CombatAction e) {
        if (e.evt == "DamageAttempt_CastCollision") {
            // Debug.Log("Possibility of applying dmg after collision " + e.source + " to " + e.target);
            e.evt = "Damage";
        }
        if (e.evt == "Damage") {
            if (e.target) {
                Debug.Log("Applying dmg from " + e.source + " to " + e.target+" w amt "+e.source.data.ability1_dmg);
                e.target.Damage(e.source.data.ability1_dmg);
            }
        }
    }

    internal static void ProcessPhysicsAction(CombatAction e) {
        if (e.evt == "FixedUpdate_MoveByDirection" && e.source) {
            e.source.unity.rig.MovePosition(e.source.transform.position + e.direction);
        }
    }
}
