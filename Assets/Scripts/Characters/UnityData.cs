using TMPro;
using UnityEngine;

[System.Serializable]
public class UnityData {
    public Rigidbody2D rig;
    public TextMeshProUGUI combatMessages;

    public void UpdateCombatMsg(RTCharacterData data) {
        if (combatMessages == null) return;

        combatMessages.text = 
            (data.isStunned > 0 ? "(Stunned)" : "") + 
            data.msg;
    }
}
