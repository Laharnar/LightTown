using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class AbilityData {
    public bool avaliable = true;
    public float ability1_radius = 1;
    public float ability1_reachDistance = 1;
    public int ability1_dmg = 1;

    public float rangeLimit=1;
    public Stun stun;

}
[System.Serializable]
public class CharacterData {
    public int alliance = 0;
    public float moveSpeed = 1;
    public int maxHp = 10;

    [HideInInspector]public AbilityData[] abilities;

    [Header("AI")]
    public bool ai = false;
    public float ai_atkDistance;
}
[System.Serializable]
public class RTCharacterData {
    public Vector2 move;
    public bool shouldAttack;
    public Transform target;
    public bool canMove;
    public int curHp = 10;
    public Vector2 lastMove;
    public bool isStunned;

    public void Init(CharacterData data) {
        curHp = data.maxHp;
        lastMove = Vector2.right;
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

    // exprimental
    public ProcessingLimits abilityCombatLimits;
    private int activeAbility = 0;
    [SerializeField] internal List<DecoratorHolder> abilities = new List<DecoratorHolder>();

    private void Start() {
        // loading abilities
        data.abilities = new AbilityData[abilities.Count];
        for (int i = 0; i < abilities.Count; i++) {
            data.abilities[i] = abilities[i].evt;
        }

        // init
        realtime.Init(data);

        GameAI.RegisterUnit(this);

        // experimental.
        StartCoroutine(CharacterLimits());
    }

    // experimental.
    private IEnumerator CharacterLimits() {
        while (true) {
            if (abilityCombatLimits.waitBetweenIssuingAbilities > 0) {
                yield return new WaitForSeconds(abilityCombatLimits.waitBetweenIssuingAbilities);
                if (abilityCombatLimits.waitStun > 0) {
                    Debug.Log("waiting stun ou t ");
                    realtime.isStunned = false;
                    yield return new WaitForSeconds(abilityCombatLimits.waitStun);
                    realtime.isStunned = true;
                }
                abilityCombatLimits.time_waitBetweenIssuingAbilities = Time.time;
                abilityCombatLimits.waitStun = 0;
                abilityCombatLimits.ready = true;

            } else yield return null;
        }
    }
    private void FixedUpdate() {
        if (data.ai == false) {
            unity.rig.MovePosition((Vector2)transform.position + realtime.move * data.moveSpeed * Time.fixedDeltaTime);
        }
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

    void Action(Vector2 dir, bool attack) {
        if (attack ) {
            if (abilityCombatLimits.ready) {
                int abilityId = 0;
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, data.abilities[abilityId].ability1_radius, realtime.lastMove, data.abilities[abilityId].ability1_reachDistance);
                //Debug.Log("Attempting atk "+hits.Length);
                // use event processor?
                int atkId = this.activeAbility;
                for (int i = 0; i < hits.Length; i++) {
                    if (hits[i].transform.root != transform.root) { // rebuild combat adding pipeline
                        GameManager.instance.combatProcessor.Add(
                            new CombatAction(CombatActionId.DamageHostilesAttempt_CastCollision, this, hits[i].transform.GetComponent<Character01>(),
                                atkId, Vector2.zero)
                            );// active ability
                    }
                }
                abilityCombatLimits.ready = false;

                /*//player specific reset. otherwise space only hold one frame
                if (!data.ai) {
                    realtime.shouldAttack = false;
                }*/
            }
        }
        if (data.ai) {
            if (realtime.canMove && !realtime.isStunned) {
                // add move into combat processor too?
                GameManager.instance.combatProcessor.Add(
                    new CombatAction(CombatActionId.FixedUpdate_MoveByDirection, this, null, -1, realtime.move * data.moveSpeed * Time.fixedDeltaTime));
            } else {
                GameManager.instance.combatProcessor.Add(
                    new CombatAction(CombatActionId.FixedUpdate_MoveByDirection, this, null, -1, Vector2.zero));
            }
        }
    }

    void Player() {

        //realtime.shouldAttack = false;
        realtime.move = Vector2.zero;
        realtime.canMove = true;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        realtime.shouldAttack = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0);
        realtime.move = new Vector2(h, v);

        if (realtime.move!=Vector2.zero)
            realtime.lastMove = realtime.move;
    }

    void AllyAI() {
        int alliance = 1;
        ExecApproachTillDistAI(alliance);
       

    }

    private void ExecApproachTillDistAI(int alliance) {
        realtime.shouldAttack = false;
        realtime.move = Vector2.zero;
        realtime.canMove = true;

        Character01 unitTarget = GameAI.FindUnit(transform.position, alliance);

        if (unitTarget) {
            Vector2 target = unitTarget.transform.position;// find ally
            realtime.move = target - (Vector2)transform.position;
            realtime.target = unitTarget.transform;

            CheckForAbility(target, data.abilities[0]);

            realtime.lastMove = realtime.move;
        }
    }

    void CheckForAbility(Vector2 targetPos, AbilityData ability) {
        if (Vector2.Distance(transform.position, targetPos) < ability.rangeLimit) {
            realtime.shouldAttack = true;
            realtime.canMove = false;
        }
    }

    void EnemyAI() {
        int alliance = 0;
        ExecApproachTillDistAI(alliance);
    }

    internal void Damage(int ability1_dmg) {
        realtime.curHp -= ability1_dmg;

        if (realtime.curHp < 0) {
            GameAI.DestroyUnit(this);
            Destroy(gameObject);
        }
    }

}
