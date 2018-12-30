using System;
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

        // handle events
        if (e.evt == CombatActionId.Damage) {
            // Note: Doesn't support abilities to be applied without source, after death(buffs...).
            if (PassFiltering(e.source.data.abilities[e.abilityId].Last.Value.targetFilter, e)) {
                int dmg = activatedAbility.GetDmg();
                Debug.Log("Applying dmg from " + e.source + " to " + e.target+" w amt "+ dmg);
                e.target.Damaged(e, dmg);
            }
        }
    }

    // todo: export combat filtering class.
    public static bool PassFiltering(TargetFilter filter, CombatAction e) {
        return (filter == TargetFilter.Enemies && e.target && e.source.data.alliance != e.target.data.alliance)
            || (filter == TargetFilter.All)
            || (filter == TargetFilter.AlliesAll && e.target && e.source.data.alliance == e.target.data.alliance)
            || (filter == TargetFilter.AlliesOther && e.target && e.target != e.source && e.source.data.alliance == e.target.data.alliance)
            || (filter == TargetFilter.Self && e.target && e.target == e.source);
    }

    internal static void ProcessPhysicsAction(CombatAction e) {
        if (e.evt == CombatActionId.FixedUpdate_MoveByDirection && e.source) {
            e.source.unity.rig.MovePosition(e.source.transform.position + e.direction);
        }
    }
}
