using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityComboTree {
    public AbilityComboItem[] items;

    public string GetTagOfNextAbility(LinkedList<AbilityData> curItem) {
        bool listAccess = true; // true: pick first avaliable. false: last avaliable.
        AbilityData data = curItem.Last.Value;
        AbilityComboItem item = GetComboItemByTag(data.abilityTag);
        if (item == null) {
            return "Failed to choose";
        }

        // pick next
        string next = item.Default;
        for (int i = 0; i < item.NumOfPaths; i++) {
            if (item.ConditionPass(i)) {
                next = item.Get(i);
                if (listAccess) { // stop execution if it's First item.
                    break;
                }
            }
        }
        return next;
    }

    internal int GetAbilityByTag(string tag) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i].tag == tag) {
                return i;
            }
        }
        Debug.Log("Ability with NAME doesn exist. " + tag);
        return -1;
    }

    private AbilityComboItem GetComboItemByTag(string abilityTag) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i].tag == abilityTag) {
                return items[i];
            }
        }
        return null;
    }
}
