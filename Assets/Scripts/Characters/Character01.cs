using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class AbilityData {
    public string abilityName = "Unnamed";
    public bool avaliable = true;
    public float ability1_radius = 1;
    public int ability1_dmg = 1;

    public float rangeLimit=1;
    public Stun stun;

    
}
[System.Serializable]
public class CharacterData {
    public int alliance = 0;
    public float moveSpeed = 1;
    public int maxHp = 10;

    // expeimental cycle 2. saved from prefabs
    public LinkedList<AbilityData>[] abilities;

    [Header("AI")]
    public bool ai = false;
}
[System.Serializable]
public class RTCharacterData {
    public Vector2 move;
    public bool shouldAttack;
    public Transform target;
    public bool canMove;
    public int curHp = 10;
    public Vector2 lastMove;
    public int isStunned;

    // experimental - cycle 2
    // separated handling of when abilities are activated and when they end.
    public Dictionary<Character01, Coroutine> lastActivatedAbility = new Dictionary<Character01, Coroutine>();

    public void Init(CharacterData data) {
        curHp = data.maxHp;
        lastMove = Vector2.right;
    }

    // experimental - cycle 2
    // doesn't support movement.
    public bool RecordAbility(Character01 character, LinkedList<AbilityData> data, CombatAction action) {
        if (character == null || data == null) {
            Debug.Log("Null data."); return false;
        }
        if (!lastActivatedAbility.ContainsKey(character)) {
            lastActivatedAbility.Add(character, null);
        }
        if (lastActivatedAbility[character] == null) {
            lastActivatedAbility[character] = character.StartCoroutine(character.AbilityCycle(data, action));
            return true;
        }
        return false;
    }

    public void AbilityDone(Character01 character) {
        lastActivatedAbility[character] = null;
    }
}

public class Character01 : MonoBehaviour
{
    public CharacterData data;
    /// <summary>
    /// rt info
    /// </summary>
    public RTCharacterData rt;
    public UnityData unity;

    // exprimental
    public ProcessingLimits combatLimits;
    private int activeAbility = 0;
    [SerializeField] internal List<DecoratorHolder> abilities = new List<DecoratorHolder>();

    // experimental, cycle 2 - status
    Coroutine status;

    private void Start() {
        // loading abilities
        data.abilities = new LinkedList<AbilityData>[abilities.Count];
        for (int i = 0; i < abilities.Count; i++) {
            data.abilities[i] = DecoratorHolder.ConstructAbilityStack(abilities[i]);//.evt
        }

        // init
        rt.Init(data);

        GameAI.RegisterUnit(this);

        // experimental.
        GameManager.instance.StartCoroutine(CharacterLimits());
    }
    // experimental 2 - cycle 2.
    /// <summary>
    /// Wait out single ability.
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
                if (node.Value.stun.stunLength > 0) {
                    GameManager.instance.StartCoroutine(action.target.Stunned(action, node.Value.stun.stunLength));
                }

                //Damaged(action, node.Value.ability1_dmg);
                GameManager.instance.combatProcessor.Add(action);
                //yield return new WaitForSeconds(combatLimits.delayAfterAttacking);
                // yield return StartCoroutine(CombatProcessing.ProcessAction());

                node = node.Next;
            } while (node != null);
        }
        yield return null;
        rt.AbilityDone(this);
    }


    // experimental.
    private IEnumerator CharacterLimits() {
        while (true) {
            if (combatLimits.delayAfterAttacking > 0) {
                yield return new WaitForSeconds(combatLimits.delayAfterAttacking);

                // pick attack.
                /*if (combatLimits.waitStun > 0) {
                    Debug.Log("waiting stun ou t ");
                    rt.isStunned ++;
                    yield return new WaitForSeconds(combatLimits.waitStun);
                    rt.isStunned --;
                }*/
                //combatLimits.waitStun = 0;
                combatLimits.ready = true;

            } 
            else yield return null;
        }
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

        
    }

    void Action(Vector2 dir, bool attack) {
        if (attack) {
            if (combatLimits.ready) {
                int abilityId = 0;
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, data.abilities[abilityId].Last.Value.ability1_radius, rt.lastMove, data.abilities[abilityId].Last.Value.rangeLimit);
                //Debug.Log("Attempting atk "+hits.Length);
                // use event processor?
                BeginNewAbility(this.activeAbility, hits);
                

                /*//player specific reset. otherwise space only hold one frame
                if (!data.ai) {
                    rt.shouldAttack = false;
                }*/
            }
        }
        if (data.ai) {
            if (rt.canMove && rt.isStunned == 0) {
                // add move into combat processor too?
                GameManager.instance.combatProcessor.Add(
                    new CombatAction(CombatActionId.FixedUpdate_MoveByDirection, this, null, -1, rt.move * data.moveSpeed * Time.fixedDeltaTime));
            } else {
                GameManager.instance.combatProcessor.Add(
                    new CombatAction(CombatActionId.FixedUpdate_MoveByDirection, this, null, -1, Vector2.zero));
            }
        }
    }

    private void BeginNewAbility(int activeAbility, RaycastHit2D[] hits) {
        //Debug.Log(name+ " "+hits.Length);
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].transform.root != transform.root) { // rebuild combat adding pipeline
                Character01 target = hits[i].transform.GetComponent<Character01>();
                // maybe 1 step too much?
                rt.RecordAbility(this, data.abilities[activeAbility], new CombatAction(CombatActionId.DamageAttempt_CastCollision, this, target, activeAbility, Vector3.zero));

                
            }
        }
        combatLimits.ready = false;
    }

    void Player() {

        //rt.shouldAttack = false;
        rt.move = Vector2.zero;
        rt.canMove = true;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rt.shouldAttack = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0);
        rt.move = new Vector2(h, v);

        if (rt.move!=Vector2.zero)
            rt.lastMove = rt.move;
    }

    void AllyAI() {
        int alliance = 1;
        ExecApproachTillDistAI(alliance);
    }

    private void ExecApproachTillDistAI(int alliance) {
        rt.shouldAttack = false;
        rt.move = Vector2.zero;
        rt.canMove = true;

        Character01 unitTarget = GameAI.FindUnit(transform.position, this, 
            (source, target) => target.data.alliance != source.data.alliance);

        if (unitTarget) {
            Vector2 target = unitTarget.transform.position;// find ally
            rt.move = target - (Vector2)transform.position;
            rt.target = unitTarget.transform;

            CheckForAbility(target, data.abilities[0].Last.Value);

            rt.lastMove = rt.move;
        }
    }

    void CheckForAbility(Vector2 targetPos, AbilityData ability) {
        if (Vector2.Distance(transform.position, targetPos) < ability.rangeLimit) {
            rt.shouldAttack = true;
            rt.canMove = false;
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
    #region STATUSES
    public void Damaged(CombatAction a, int value) {
        a.target.Damage(value);
    }

    public IEnumerator Stunned(CombatAction a, float time) {
        a.target.rt.isStunned ++;
        yield return new WaitForSeconds(time);
        a.target.rt.isStunned --;
    }
    // TODO : untested
    public IEnumerator Poisoned(CombatAction a, float stunLength, int value, float updateRate = 1) {
        float sum = 0;
        while (sum < stunLength) {
            sum += updateRate;
            if (sum < stunLength)
                yield return new WaitForSeconds(updateRate);
            else yield return new WaitForSeconds(sum % stunLength);

            a.target.Damage(value);
        }
    }
    #endregion
}