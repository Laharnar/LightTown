﻿using System;
using UnityEngine;

public static class CombatProcessing {
    // edit depending on game.
    public static void ProcessAction(CombatAction e) {
        AbilityData activatedAbility = e.source.data.abilities[e.abilityId].Last.Value;
        // filtering
        if (e.evt == CombatActionId.DamageAttempt_CastCollision) {
            // Debug.Log("Possibility of applying dmg after collision " + e.source + " to " + e.target);
            e.evt = CombatActionId.Damage;
        }
        if (e.evt == CombatActionId.DamageHostilesAttempt_CastCollision) {
            // Debug.Log("Possibility of applying dmg after collision " + e.source + " to " + e.target);
            e.evt = CombatActionId.DamageHostiles;
        }

        // handle events
        if (e.evt == CombatActionId.Damage) {
            if (e.target) {
                Debug.Log("Applying dmg from " + e.source + " to " + e.target+" w amt "+activatedAbility.ability1_dmg);
                e.target.Damaged(e, activatedAbility.ability1_dmg);
            }
        }
        if (e.evt == CombatActionId.DamageHostiles) {
            if (e.target && e.source.data.alliance != e.target.data.alliance) {
                Debug.Log("Applying dmg from " + e.source + " to " + e.target + " w amt " + activatedAbility.ability1_dmg);
                e.target.Damaged(e, activatedAbility.ability1_dmg);
            }
        }
    }

    internal static void ProcessPhysicsAction(CombatAction e) {
        if (e.evt == CombatActionId.FixedUpdate_MoveByDirection && e.source) {
            e.source.unity.rig.MovePosition(e.source.transform.position + e.direction);
        }
    }
}
