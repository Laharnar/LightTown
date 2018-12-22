using UnityEngine;
[System.Serializable]
public abstract class IDecorator {

}
[System.Serializable]
public class AbilityDecorator: IDecorator {
    protected IDecorator action;

    public AbilityDecorator(IDecorator action) {
        this.action = action;
    }
    
}
