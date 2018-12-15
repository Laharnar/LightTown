using System.Collections.Generic;
using UnityEngine;

public class GameAI  {
    public static GameAI instance;

    public List<Character01> units = new List<Character01>();

    public GameAI() {
        if (instance == null)
            instance = this;
        else Debug.Log("Singleton exists. - GameAI");
    }

    public static void RegisterUnit(Character01 unit) {
        instance.units.Add(unit);
    }
    public static void DestroyUnit(Character01 unit) {
        instance.units.Remove(unit);
    }
    /// <summary>
    /// Skips units on same postions as parameter.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="alliance"></param>
    /// <returns></returns>
    internal static Character01 FindUnit(Vector2 position, int alliance) {
        List<Character01> units = instance.units;
        float minDist = float.MaxValue;
        int minI = -1;
        for (int i = 0; i < units.Count; i++) {
            if (units[i].data.alliance == alliance) {
                float dist = Vector2.Distance(position, units[i].transform.position);
                if (dist  > 0 && (minI == -1 || dist < minDist)) {
                    minI = i;
                    minDist = dist;
                }
            }
        }
        if (minI != -1)
            return units[minI];
        return null;
    }
}