using UnityEngine;

public class CombatAction {

    public string evt;
    public Character01 source;
    public Character01 target;
    public int abilityId;
    public Vector3 direction;

    public CombatAction(string evt, Character01 source, Character01 target, int abilityId, Vector3 direction) {
        this.evt = evt;
        this.source = source;
        this.target = target;
        this.abilityId = abilityId;
        this.direction = direction;
    }
}
