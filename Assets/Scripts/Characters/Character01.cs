using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class CharacterPrefabsLoad {
    [SerializeField] internal List<ScriptableObject> abilities = new List<ScriptableObject>();

    public AbilityTreeSetup tree;

    public DecoratorHolder followAllyAbilityData;

}
public class Character01 : MonoBehaviour
{
    public CharacterData data;
    /// <summary>
    /// rt info
    /// </summary>
    public RTCharacterData rt;
    public UnityData unity;

    // experimental
    public ProcessingLimits combatLimits;
    public CharacterPrefabsLoad prefs;



    private void Start() {
        data.InitWithUnityData(prefs);
        
        // init
        rt.Init(data);

        GameAI.RegisterUnit(this);

        GameManager.instance.StartCoroutine(combatLimits.CharacterLimits());
    }
    private void FixedUpdate() {
        if (data.ai == false) {
            unity.rig.MovePosition((Vector2)transform.position + rt.move * data.moveSpeed * Time.fixedDeltaTime);
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
        rt.move.Normalize();
        Action(rt.move, rt.shouldAttack);

        unity.UpdateCombatMsg(rt);   
    }

    void Action(Vector2 dir, bool attack) {
        if (attack && combatLimits.ready) {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position,
                data.abilities[rt.activeAbilityId].Last.Value.ability1_radius, rt.lastNonZeroMove,
                data.abilities[rt.activeAbilityId].Last.Value.rangeLimit);
            //Debug.Log("Attempting atk "+hits.Length);
            BeginNewAbility(hits);

            //player specific reset. otherwise space only hold one frame
            /*if (!data.ai) {
                rt.shouldAttack = false;
            }*/
        }

        if (data.ai) {
            if (rt.isMoving && rt.isStunned == 0) {
                // add move into combat processor too?
                GameManager.instance.combatProcessor.Add(
                    new CombatAction(CombatActionId.FixedUpdate_MoveByDirection, this, null, -1, rt.move * data.moveSpeed * Time.fixedDeltaTime));
            } else {
                GameManager.instance.combatProcessor.Add(
                    new CombatAction(CombatActionId.FixedUpdate_MoveByDirection, this, null, -1, Vector2.zero));
            }

        }
        if (!data.ai) {
            if (rt.rallyingCall) {
                if (rt.activeFollower) {
                    rt.DetachFollower();
                } else {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, data.rallyRange);
                    for (int i = 0; i < colliders.Length; i++) {
                        Character01 c = colliders[i].GetComponent<Character01>();
                        if (c.IsAlly(this)) {
                            c.AttachAsFollowerTo(this);
                            break;
                        }
                    }
                    rt.rallyingCall = false;
                }
            }
        }
    }

    // from follower
    public void AttachAsFollowerTo(Character01 target) {
        target.rt.activeFollower = this;
        rt.following = target;
    }

    public bool IsAlly(Character01 character01) {
        return data.alliance == character01.data.alliance && character01 != this;
    }

    public bool IsEnemy(Character01 character01) {
        return data.alliance != character01.data.alliance;
    }

    private void BeginNewAbility(RaycastHit2D[] hits) {
        // pick ability based on current combo
        if (rt.lastAbility != null) {
            rt.activeAbilityId = data.GetNextAbility(rt.lastAbility);
        }

        // note: currently combat actions support 1 ability activation per target.
        rt.lastAbility = data.abilities[rt.activeAbilityId];
        Debug.Log("Attacking with ability: " + rt.lastAbility.Last.Value.abilityName+" "+rt.lastAbility.Last.Value.abilityTag + " " + hits.Length);
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].transform.root != transform.root) { // rebuild combat adding pipeline
                Character01 target = hits[i].transform.GetComponent<Character01>();

                rt.RecordAbilityHit(this, data.abilities[rt.activeAbilityId], 
                    new CombatAction(CombatActionId.DamageAttempt_CastCollision, this, 
                    target, rt.activeAbilityId, Vector3.zero));
                
            }
        }
        combatLimits.ready = false;
    }

    void Player() {

        //rt.shouldAttack = false;
        rt.move = Vector2.zero;
        rt.isMoving = true;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rt.shouldAttack = Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J);
        rt.rallyingCall = Input.GetKeyDown(KeyCode.Space);
        rt.move = new Vector2(h, v);

        if (rt.move!=Vector2.zero)
            rt.lastNonZeroMove = rt.move;
    }

    void AllyAI() {

        if (data.ai)
            rt.rallyingCall = false;
        int alliance = 1;
        ExecApproachTillDistAI(alliance);
    }

    private void ExecApproachTillDistAI(int alliance) {
        rt.rallyingCall = false;
        rt.shouldAttack = false;
        rt.move = Vector2.zero;
        rt.isMoving = true;

        // select unit to attack
        Vector2 targetPos = transform.position;// find ally
        Character01 unitTarget=null;
        if (rt.following) {
            targetPos = rt.following.transform.position;// find ally
        } else {
            unitTarget = GameAI.FindUnit(transform.position, this,
                (source, target) => target.data.alliance != source.data.alliance);
            if (unitTarget != null) {
                rt.target = unitTarget.transform;
                targetPos = unitTarget.transform.position;// find ally
            }
        }
        // AI
        rt.move = targetPos - (Vector2)transform.position;

        if (rt.following == null) {
            CheckForAbility(targetPos, data.abilities[0].Last.Value);
        } else {
            NonOffensiveAbility(targetPos, data.followAllyAbility);
        }

        rt.lastNonZeroMove = rt.move;
    }

    void NonOffensiveAbility(Vector2 targetPos, AbilityData ability) {
        if (Vector2.Distance(transform.position, targetPos) < ability.rangeLimit) {
            rt.shouldAttack = false;
            rt.isMoving = false;
        }
    }

    void CheckForAbility(Vector2 targetPos, AbilityData ability) {
        if (Vector2.Distance(transform.position, targetPos) < ability.rangeLimit) {
            rt.shouldAttack = true;
            rt.isMoving = false;
        }
    }

    void EnemyAI() {
        int alliance = 0;
        ExecApproachTillDistAI(alliance);
    }

    internal void Damage(int ability1_dmg) {
        rt.curHp -= ability1_dmg;

        if (rt.curHp < 0) {
            GameAI.DestroyUnit(this);
            Destroy(gameObject);
        }
    }



    // experimental 2 - cycle 2.
    /// <summary>
    /// Wait out single ability, over single target
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    public IEnumerator AbilityCycle(LinkedList<AbilityData> ability, CombatAction action) {
        Debug.Log("running ability cycle." + action.source);
        //Note: source is NOT necessarily this object, even if function is local.
        //atm it is.
        LinkedListNode<AbilityData> node = ability.First;
        if (node != null) {
            do {
                // Filtering is limited to last ability's value.
                if (CombatProcessing.PassFiltering(ability.Last.Value.targetFilter, action)) {
                    if (node.Value.stun.stunLength > 0) {
                        GameManager.instance.StartCoroutine(action.Stunned(action, node.Value.stun.stunLength));
                    }
                }
                
                //Damaged(action, node.Value.ability1_dmg);
                GameManager.instance.combatProcessor.Add(action);
                node = node.Next;
            } while (node != null);
        }
        yield return null;
        rt.AbilityDone();
    }
}