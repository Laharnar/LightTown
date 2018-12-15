using System.Collections.Generic;
using UnityEngine;

public class CombatProcessor:MonoBehaviour {

    Queue<CombatAction> actions = new Queue<CombatAction>();
    Dictionary<Transform, CombatAction> fixUpdateActions = new Dictionary<Transform, CombatAction>();

    public void Add(CombatAction combatAction) {
        actions.Enqueue(combatAction);
    }

    void Update() {
        while (actions.Count > 0) {
            CombatAction action = actions.Dequeue();

            // filter out fixed update actions.
            if (action.evt == CombatActionId.FixedUpdate_MoveByDirection && action.source) {
                ConvertUpdateToFixedUpdateProcessing(action);
                continue;
            }

            CombatProcessing.ProcessAction(action);

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
                CombatProcessing.ProcessPhysicsAction(item.Value);

                //fixUpdateActions[item.Key] = null;
            }
        }
    }
}
