using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData {
    public int alliance = 0;
    public float moveSpeed = 1;
    public int maxHp = 10;

    // expeimental cycle 2. saved from prefabs
    public LinkedList<AbilityData>[] abilities;
    // expermental - cycle 3
    // which abilities lead where, + ability state. don't change values in tree directly.
    public AbilityComboTree abilityTree;

    public float rallyRange = 2;

    [Header("AI")]
    public bool ai = false;
    public AbilityData followAllyAbility;

    


    public void InitWithUnityData(CharacterPrefabsLoad prefs) {
        // load abilities into list
        abilities = new LinkedList<AbilityData>[prefs.abilities.Count];
        for (int i = 0; i < prefs.abilities.Count; i++) {
            abilities[i] = DecoratorHolder.ConstructAbilityStack((DecoratorHolder)prefs.abilities[i]);//.evt
        }

        abilityTree = prefs.tree.tree;

        followAllyAbility = prefs.followAllyAbilityData.evt;
    }

    internal int GetNextAbility(LinkedList<AbilityData> lastAbility) {
        return abilityTree.GetAbilityByTag(abilityTree.GetTagOfNextAbility(lastAbility))
    }
}
