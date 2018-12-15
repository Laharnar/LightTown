using System;
using UnityEngine;
public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public GameAI gameAi;
    public CombatProcessor combatProcessor;

    private void Awake() {
        if (instance == null) {
            instance = this;
            gameAi = new GameAI();
        }
        else Debug.Log("Singleton exists. - GameManager");
    }

}
