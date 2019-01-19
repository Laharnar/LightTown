using UnityEngine;

[System.Serializable]
public class AnimationBool {
    public string boolName;
    public bool value = false;
}

[System.Serializable]
public class AnimationTrigger {
    public string triggerName = "";
}

[System.Serializable]
public class AnimationInt {
    public string intName = "";
    public int value;
}
[System.Serializable]
public class AnimationInfo {
    public AnimationInt aint;
    public AnimationTrigger atrigger;
    public AnimationBool abool;
}
[System.Serializable]
public class AnimationActivation {

    // activation of triggers, bools, etc.
    AnimationInfo whatToActivate;
    // one way to end this is

}
public class AnimationTransition { 

    // activation of events. -- all unit copies use same animation!
    void InitAnimationWithCallback(Animator character, string animationName, AnimationEvent evt) {

    }
    
    void OnEndCallback() {

    }
}

[System.Serializable]
public class AbilityData {
    public string abilityName = "Unnamed";
    public string abilityTag = "UndefinedTag";
    public bool avaliable = true;
    public float ability1_radius = 1;
    public int dmgMin = 1;
    public int dmgMax = 1;

    public AnimationInfo runAnimations;
    public string runAnimation = "";

    /// <remarks>Taken only for the last(base) item in chain.</remarks>
    public TargetFilter targetFilter = TargetFilter.Enemies;

    public float rangeLimit=1;
    public Stun stun;

    // TODO: implement.
    public bool useDelay = false;
    public float delayAfterAttack = 1f;

    /// <summary>
    /// What has to be true for ability to activate.
    /// </summary>
    /// <remarks>Condition is take only for the last(base) item in chain.</remarks>
    public AbilityCondition condition;

    internal int GetDmg() {
        return UnityEngine.Random.Range(Mathf.Min(dmgMin, dmgMax), Mathf.Max(dmgMin, dmgMax)) ;
    }
    
}
