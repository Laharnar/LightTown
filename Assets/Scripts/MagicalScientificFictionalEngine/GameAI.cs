using System;
using System.Collections.Generic;
using UnityEngine;

public class GameAI {
    public static GameAI Instance;

    public List<Character01> Units = new List<Character01>();

    public GameAI() {
        if (Instance == null)
            Instance = this;
        else Debug.Log("Singleton exists. - GameAI");
    }

    public static void RegisterUnit(Character01 unit) {
        Instance.Units.Add(unit);
    }
    public static void DestroyUnit(Character01 unit) {
        Instance.Units.Remove(unit);
    }
    /// <summary>
    /// Skips units on same postions as parameter.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="alliance"></param>
    /// <returns></returns>
    internal static Character01 FindUnit(Vector2 position, int alliance) {
        List<Character01> units = Instance.Units;
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

    internal static Character01 FindEnemyUnit(Vector2 position, int alliance) {
        List<Character01> units = Instance.Units;
        float minDist = float.MaxValue;
        int minI = -1;
        for (int i = 0; i < units.Count; i++) {
            if (units[i].data.alliance != alliance) {
                float dist = Vector2.Distance(position, units[i].transform.position);
                if (dist > 0 && (minI == -1 || dist < minDist)) {
                    minI = i;
                    minDist = dist;
                }
            }
        }
        if (minI != -1)
            return units[minI];
        return null;
    }

    internal static Character01 FindUnit(Vector2 position, Character01 source, Func<Character01, Character01, bool> check) {
        List<Character01> units = Instance.Units;
        float minDist = float.MaxValue;
        int minI = -1;
        for (int i = 0; i < units.Count; i++) {
            if (source == units[i]) continue;
            if (check(source, units[i])) {//units[i].data.alliance != alliance
                float dist = Vector2.Distance(position, units[i].transform.position);
                if (dist > 0 && (minI == -1 || dist < minDist)) {
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