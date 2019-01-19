using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RTCharacterData {

    public int curHp = 10;
    public int isStunned;

    public bool isMoving;
    public Vector2 move;
    public Vector2 lastNonZeroMove;

    public bool shouldAttack;
    public Transform target;

    public int activeAbilityId = 0;
    public LinkedList<AbilityData> lastAbility;

    // experimental - cycle 2
    // Handling of when abilities are activated and when they end, is separate.
    private Coroutine lastAbilityCycle;

    public string combatUiMsg;

    public bool rallyingCall;
    public Character01 activeFollower;
    public Character01 following;

    public void Init(CharacterData data) {
        curHp = data.maxHp;
        lastNonZeroMove = Vector2.right;

    }

    // experimental - cycle 2
    /// <summary>
    /// Saves last ability for later use.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="data"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool RecordAbilityHit(Character01 character, LinkedList<AbilityData> data, CombatAction action) {
        if (character == null || data == null) {
            Debug.Log("Null data."); return false;
        }
        Debug.Log("Actitivating ability on character. against target. "+character.name +"; "+ data.Last.Value.abilityName+"; "+ action.target.name);
        lastAbilityCycle = character.StartCoroutine(character.AbilityCycle(data, action));
        return true;
    }

    public void AbilityDone() {
        lastAbilityCycle = null;
    }

    // from target
    public void DetachFollower() {
        activeFollower.rt.following = null;
        activeFollower = null;
    }
    // from follower
    public void DetachFromTarget() {
        following.rt.activeFollower = null;
        following = null;
    }
    
}
