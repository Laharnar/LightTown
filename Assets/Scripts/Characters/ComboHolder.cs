using UnityEngine;

// don't use atm.(cycle 3)
[CreateAssetMenu(fileName = "ComboDecorator", menuName = "Ability systems/Combo", order = 1)]
public class ComboHolder:ScriptableObject {
    public DecoratorHolder[] combo;
}
