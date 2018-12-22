using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Abandoned in favor of local handling.")]
public class CombatProcessor:MonoBehaviour {

    Queue<IDecorator> actions = new Queue<IDecorator>();
    Dictionary<Transform, IDecorator> fixUpdateActions = new Dictionary<Transform, IDecorator>();

    public void Add(IDecorator combatAction) {
        actions.Enqueue(combatAction);
    }

    void Update() {
        // not used

        while (actions.Count > 0) {
            IDecorator ability = actions.Dequeue();
            CombatAction action = ability as CombatAction;
            //if (action.abilityId > -1)
                //ability = action.source.abilities[action.abilityId].AddAttributes(ability);
            // filter out fixed update actions.
            if (ability != null && action.evt == CombatActionId.FixedUpdate_MoveByDirection && action.source) {
                ConvertUpdateToFixedUpdateProcessing(action, ability);
                continue;
            }
            Debug.Log("ability " + ability.GetType() + " " + action.source + " " + action.target);

            CombatProcessing.ProcessAction(action);
            //ability.ActivateAbility();

        }
    }

    private void ConvertUpdateToFixedUpdateProcessing(CombatAction action, IDecorator ability) {
        // only keep 1 update per transform to ensure proper refresh rate between update and fixed update.
        if (!fixUpdateActions.ContainsKey(action.source.transform)) {
            fixUpdateActions.Add(action.source.transform, null);
        } else {
            fixUpdateActions[action.source.transform] = ability;
        }
    }

    private void FixedUpdate() {
        foreach (var item in fixUpdateActions) {
            if (item.Value!= null && item.Value.GetType() == typeof(CombatAction)) {
                CombatProcessing.ProcessPhysicsAction(item.Value as CombatAction);

                //fixUpdateActions[item.Key] = null;
            }
        }
    }
}
