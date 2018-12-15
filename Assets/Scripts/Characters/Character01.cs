using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData {
    public int alliance = 0;
    public float moveSpeed = 1;
    public int maxHp = 10;

    [Header("Abilities")]
    public float ability1_radius = 1;
    public float ability1_reach = 1;
    public int ability1_dmg = 1;

    [Header("AI")]
    public bool ai = false;
    public float ai_atkDistance;
}
[System.Serializable]
public class RTCharacterData {
    public Vector3 move;
    public bool shouldAttack;
    public Transform target;
    public bool canMove;
    public int curHp = 10;

    public void Init(CharacterData data) {
        curHp = data.maxHp;
    }
}
[System.Serializable]
public class UnityData {
    public Rigidbody2D rig;
}

public class Character01 : MonoBehaviour
{
    public CharacterData data;
    public RTCharacterData realtime;
    public UnityData unity;

    private void Start() {
        realtime.Init(data);

        GameAI.RegisterUnit(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (data.ai) {
            if (data.alliance==0)
                AllyAI();
            else if (data.alliance == 1)
                EnemyAI();
        } else {
            Player();
        }
        realtime.move.Normalize();
        Action(realtime.move, realtime.shouldAttack);
    }

    private void FixedUpdate() {
       
    }

    void Action(Vector2 dir, bool attack) {
        if (attack) {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, data.ability1_radius, realtime.move, data.ability1_reach);
            //Debug.Log("Attempting atk "+hits.Length);
            // use event processor?
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].transform.root != transform.root) {
                    GameManager.instance.combatProcessor.Add(new CombatAction("DamageAttempt_CastCollision", this, hits[i].transform.GetComponent<Character01>(),
                        0, Vector2.zero));// active ability
                }
            }
        }
        if (realtime.canMove) {
            // add move into combat processor too?
            GameManager.instance.combatProcessor.Add(
                new CombatAction("FixedUpdate_MoveByDirection", this, null, 0, realtime.move * data.moveSpeed * Time.fixedDeltaTime));
        } else {
            GameManager.instance.combatProcessor.Add(
                new CombatAction("FixedUpdate_MoveByDirection", this, null, 0, Vector2.zero));
        }
    }

    void Player() {

        realtime.shouldAttack = false;
        realtime.move = Vector2.zero;
        realtime.canMove = true;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        realtime.shouldAttack = Input.GetKeyDown(KeyCode.Space | KeyCode.Mouse0);
        realtime.move = new Vector2(h, v);
    }

    void AllyAI() {
        int alliance = 1;
        realtime.shouldAttack = false;
        realtime.move = Vector2.zero;
        realtime.canMove = true;

        Character01 unitTarget = GameAI.FindUnit(transform.position, alliance);

        if (unitTarget) {
            Vector2 target = unitTarget.transform.position;// find ally
            realtime.move = target - (Vector2)transform.position;
            realtime.target = unitTarget.transform;

            if (Vector2.Distance(transform.position, target) < data.ai_atkDistance) {
                realtime.shouldAttack = true;
                realtime.canMove = false;
            }
        }
    }

    void EnemyAI() {
        int alliance = 0;
        realtime.shouldAttack = false;
        realtime.move = Vector2.zero;
        realtime.canMove = true;

        Character01 unitTarget = GameAI.FindUnit(transform.position, alliance);

        if (unitTarget) {
            Vector2 target = unitTarget.transform.position;// find ally
            realtime.move = target - (Vector2)transform.position;
            realtime.target = unitTarget.transform;

            if (Vector2.Distance(transform.position, target) < data.ai_atkDistance) {
                realtime.shouldAttack = true;
                realtime.canMove = false;
            }
        }
    }

    internal void Damage(int ability1_dmg) {
        realtime.curHp -= ability1_dmg;

        if (realtime.curHp < 0) {
            GameAI.DestroyUnit(this);
            Destroy(gameObject);
        }
    }
}
