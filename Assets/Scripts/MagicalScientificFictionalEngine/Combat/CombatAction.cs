using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Which type of action is it. Used at processing.
/// </summary>
public enum CombatActionId {
    Damage,
    DamageAttempt_CastCollision,
    FixedUpdate_MoveByDirection
}
/// <summary>
/// Which type of action is it. Used at processing.
/// </summary>
public enum TargetFilter {
    Enemies,
    Self,
    AlliesOther,
    AlliesAll,
    All
}
/// <summary>
/// Combat actions are constructed based on the whole chain.
/// </summary>
public class CombatAction : IDecorator {

    public CombatActionId evt;

    public Character01 source;
    public Character01 target;

    public int abilityId;
    public Vector3 direction;

    public CombatAction(Character01 source, Character01 target) {
        this.source = source;
        this.target = target;
    }

    public CombatAction(CombatActionId evt, Character01 source, Character01 target, int abilityId, Vector3 direction) {
        this.evt = evt;
        this.source = source;
        this.target = target;
        this.abilityId = abilityId;
        this.direction = direction;
    }


    public void Damaged(CombatAction a, int value) {
        a.target.rt.combatUiMsg = ("-" + value + " " + Time.time);
        a.target.Damage(value);
    }

    public IEnumerator Stunned(CombatAction a, float time) {
        a.target.rt.isStunned++;
        yield return new WaitForSeconds(time);
        a.target.rt.isStunned--;
    }
    // TODO : untested
    public IEnumerator Poisoned(CombatAction a, float stunLength, int value, float updateRate = 1) {
        float sum = 0;
        while (sum < stunLength) {
            sum += updateRate;
            if (sum < stunLength)
                yield return new WaitForSeconds(updateRate);
            else yield return new WaitForSeconds(sum % stunLength);

            a.target.Damage(value);
        }
    }
}
