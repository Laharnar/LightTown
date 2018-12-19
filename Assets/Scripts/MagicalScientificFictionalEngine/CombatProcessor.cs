using System.Collections.Generic;
using UnityEngine;

public class CombatProcessor:MonoBehaviour {

    Queue<IDecorator> actions = new Queue<IDecorator>();
    Dictionary<Transform, IDecorator> fixUpdateActions = new Dictionary<Transform, IDecorator>();

    public void Add(IDecorator combatAction) {
        actions.Enqueue(combatAction);
    }

    void Update() {
        while (actions.Count > 0) {
            IDecorator ability = actions.Dequeue();
            CombatAction action = ability as CombatAction;
            // filter out fixed update actions.
            if (action!= null && action.evt == CombatActionId.FixedUpdate_MoveByDirection && action.source) {
                ConvertUpdateToFixedUpdateProcessing(action);
                continue;
            }

            action.source.abilities[action.abilityId].AddAttributes(ability);
            ability.ActivateAbility();

        }
    }

    private void ConvertUpdateToFixedUpdateProcessing(CombatAction action) {
        // only keep 1 update per transform to ensure proper refresh rate between update and fixed update.
        if (!fixUpdateActions.ContainsKey(action.source.transform)) {
            fixUpdateActions.Add(action.source.transform, null);
        } else {
            fixUpdateActions[action.source.transform] = action;
        }
    }

    private void FixedUpdate() {
        foreach (var item in fixUpdateActions) {
            if (item.Value!= null) {
                CombatProcessing.ProcessPhysicsAction(item.Value as CombatAction);

                //fixUpdateActions[item.Key] = null;
            }
        }
    }
}
