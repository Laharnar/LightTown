using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDecorator", menuName = "Ability systems/AbilityDecorator", order = 1)]
public class DecoratorHolder:ScriptableObject {
    public AbilityData evt;
    public int stunId = -1;
    public ScriptableObject childAction;

    public IDecorator AddAttributes(IDecorator action) {
        Debug.Log("applyin stun attribute");
        if (stunId > -1)
            return new Stun(evt.stun, action);
        return action;
    }
}
