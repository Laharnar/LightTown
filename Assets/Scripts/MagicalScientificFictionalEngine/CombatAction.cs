using System;
using UnityEngine;
/// <summary>
/// Which type of action is it. Used at processing.
/// </summary>
public enum CombatActionId {
    Damage,
    DamageAttempt_CastCollision,
    DamageHostilesAttempt_CastCollision,
    DamageHostiles,
    FixedUpdate_MoveByDirection
}

public class CombatAction : IDecorator {

    public CombatActionId evt;
    public Character01 source;
    public Character01 target;
    public int abilityId;
    public Vector3 direction;

    public CombatAction(CombatActionId evt, Character01 source, Character01 target, int abilityId, Vector3 direction) {
        this.evt = evt;
        this.source = source;
        this.target = target;
        this.abilityId = abilityId;
        this.direction = direction;
    }

    public override CombatAction ActivateAbility() {
        CombatProcessing.ProcessAction(this);
        return this;
    }
}
