using System;
using System.Collections.Generic;
using UnityEngine;
// stack is untested, but should work in theory.
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

    internal static LinkedList<AbilityData> ConstructAbilityStack(DecoratorHolder decoratorHolder) {
        // missing application of stuns, etc etc.
        return AsLinkedList(decoratorHolder);
    }

    public static LinkedList<AbilityData> AsLinkedList(DecoratorHolder start) {
        // max depth: 20 effects. that's completly over the top.
        LinkedList<AbilityData> data = new LinkedList<AbilityData>();
        data.AddLast(start.evt);
        DecoratorHolder d = start;
        for (int i = 0; i < 20 && ((DecoratorHolder)d.childAction) != null; i++) {
            data.AddLast(((DecoratorHolder)d.childAction).evt);
            d = ((DecoratorHolder)d.childAction);
        }
        return data;
    }
}
