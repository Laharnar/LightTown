[System.Serializable]
public class AbilityComboItem {
    public string tag = "Undefined";
    public string[] next;
    public AbilityCondition[] conditions;
    internal string Default { get { if(next.Length == 0){ return tag; } return next[0]; } }

    public int NumOfPaths { get { return next.Length - 1; } }

    internal string Get(int i) {
        return i < next.Length ? next[i] : "Out of range err." ;
    }

    internal bool ConditionPass(int i) {
        return true;
    }
}
