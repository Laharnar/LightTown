﻿using System;
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
}