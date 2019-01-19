using UnityEngine;

[CreateAssetMenu(fileName = "AnimationAction", menuName = "Animation systems/AnimationAction", order = 1)]
public class AnimationAction : ScriptableObject {
    public AnimationBool aBool;
    public AnimationInt aInt;
    public AnimationTrigger aTrigger;
}
