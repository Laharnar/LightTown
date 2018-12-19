using UnityEngine;

[System.Serializable]
public class Stun : AbilityDecorator {
    public float stunLength = 0;
    public Stun(Stun stun, IDecorator action) : base(action) {
        stunLength = stun.stunLength;
    }

    public override CombatAction ActivateAbility() {
        CombatAction action = base.ActivateAbility();
        Debug.Log("Applying stun from "+action.source+" to "+action.target);
        action.target.abilityCombatLimits.waitStun = stunLength;
        return action;
    }

}

