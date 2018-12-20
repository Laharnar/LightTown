using UnityEngine;
[System.Serializable]
public abstract class IDecorator {

    public abstract CombatAction ActivateAbility();
}
[System.Serializable]
public class AbilityDecorator: IDecorator {
    protected IDecorator action;

    public AbilityDecorator(IDecorator action) {
        this.action = action;
    }

    public override CombatAction ActivateAbility() {
        return action.ActivateAbility();
    }

}
