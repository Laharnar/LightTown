using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class AbilityData {
    public string abilityName = "Unnamed";
    public string abilityTag = "UndefinedTag";
    public bool avaliable = true;
    public float ability1_radius = 1;
    public int dmgMin = 1;
    public int dmgMax = 1;

    /// <remarks>Taken only for the last(base) item in chain.</remarks>
    public TargetFilter targetFilter = TargetFilter.Enemies;

    public float rangeLimit=1;
    public Stun stun;

    // TODO: implement.
    public bool useDelay = false;
    public float delayAfterAttack = 1f;

    /// <summary>
    /// What has to be true for ability to activate.
    /// </summary>
    /// <remarks>Condition is take only for the last(base) item in chain.</remarks>
    public AbilityCondition condition;

    internal int GetDmg() {
        return UnityEngine.Random.Range(Mathf.Min(dmgMin, dmgMax), Mathf.Max(dmgMin, dmgMax)) ;
    }
    
}
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
[System.Serializable]
public class AbilityComboTree {
    public AbilityComboItem[] items;

    public string GetTagOfNextAbility(LinkedList<AbilityData> curItem) {
        bool firstAvaliable = true; // true: pick first avaliable. false: last avaliable.
        AbilityData data = curItem.Last.Value;
        AbilityComboItem item = GetComboItemByTag(data.abilityTag);
        if (item!= null) {
            // pick next
            string next = item.Default;
            for (int i = 0; i < item.NumOfPaths; i++) {
                if (item.ConditionPass(i)) {
                    next = item.Get(i);
                    if (firstAvaliable) {
                        break;
                    }
                }
            }
            Debug.Log("gettin tag "+next);
            return next;
        } else {
            Debug.Log("Ability item for combos, doesn't exist. "+data.abilityName);
        }
        return "Failed to choose";
    }

    private AbilityComboItem GetComboItemByTag(string abilityTag) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i].tag == abilityTag) {
                return items[i];
            }
        }
        return null;
    }
}
[System.Serializable]
public class CharacterData {
    public int alliance = 0;
    public float moveSpeed = 1;
    public int maxHp = 10;

    // expeimental cycle 2. saved from prefabs
    public LinkedList<AbilityData>[] abilities;
    // expermental - cycle 3
    // which abilities lead where, + ability state.
    // don't change values in tree directly.
    public AbilityComboTree abilityTree;

    [Header("AI")]
    public bool ai = false;

    internal int GetAbilityByName(string tag) {
        for (int i = 0; i < abilityTree.items.Length; i++) {
            if (abilityTree.items[i].tag == tag) {
                return i;
            }
        }
        Debug.Log("Ability with NAME doesn exist. "+tag);
        return -1;
    }
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
    public int activeAbility = 0;
    public int lastAbilityId;
    public LinkedList<AbilityData> lastAbility;

    // experimental - cycle 2
    // separated handling of when abilities are activated and when they end.
    public Dictionary<Character01, Coroutine> lastActivatedAbility = new Dictionary<Character01, Coroutine>();

    public string combatUiMsg;

    public void Init(CharacterData data) {
        curHp = data.maxHp;
        lastMove = Vector2.right;
    }

    // experimental - cycle 2
    public bool RecordAbilityHit(Character01 character, LinkedList<AbilityData> data, CombatAction action) {
        if (character == null || data == null) {
            Debug.Log("Null data."); return false;
        }
        if (!lastActivatedAbility.ContainsKey(character)) {
            lastActivatedAbility.Add(character, null);
        }
        //if (lastActivatedAbility[character] == null) {
            Debug.Log("Actitivating ability on character. against target. "+character.name +"; "+ data.Last.Value.abilityName+"; "+ action.target.name);
            lastActivatedAbility[character] = character.StartCoroutine(character.AbilityCycle(data, action));
            return true;
        //}
        return false;
    }

    public void AbilityDone(Character01 character) {
        lastActivatedAbility[character] = null;
    }
}
[System.Serializable]
public class CharacterPrefabsLoad {
    [SerializeField] internal List<ScriptableObject> abilities = new List<ScriptableObject>();

    public AbilityTreeSetup tree;
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
    public CharacterPrefabsLoad prefs;

    private void Start() {
        LoadAbilities();
        LoadTree();

        // init
        rt.Init(data);

        GameAI.RegisterUnit(this);

        GameManager.instance.StartCoroutine(CharacterLimits());
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
        if (attack) {
            if (combatLimits.ready) {
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 
                    data.abilities[rt.activeAbility].Last.Value.ability1_radius, rt.lastMove, 
                    data.abilities[rt.activeAbility].Last.Value.rangeLimit);
                //Debug.Log("Attempting atk "+hits.Length);
                // use event processor?

                
                BeginNewAbility(hits);
                

                //player specific reset. otherwise space only hold one frame
                /*if (!data.ai) {
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

    private void BeginNewAbility(RaycastHit2D[] hits) {
        // pick ability based on current combo
        if (rt.lastAbility != null) {
            rt.activeAbility = data.GetAbilityByName(
                data.abilityTree.GetTagOfNextAbility(rt.lastAbility));
        }

        // note: currently combat actions support 1 ability activation per target.
        rt.lastAbility = data.abilities[rt.activeAbility];
        Debug.Log("Attacking with ability: " + rt.lastAbility.Last.Value.abilityName+" "+rt.lastAbility.Last.Value.abilityTag + " " + hits.Length);
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].transform.root != transform.root) { // rebuild combat adding pipeline
                Character01 target = hits[i].transform.GetComponent<Character01>();

                rt.RecordAbilityHit(this, data.abilities[rt.activeAbility], 
                    new CombatAction(CombatActionId.DamageAttempt_CastCollision, this, 
                    target, rt.activeAbility, Vector3.zero));

                
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

    private void LoadAbilities() {
        // load abilities into list
        data.abilities = new LinkedList<AbilityData>[prefs.abilities.Count];
        for (int i = 0; i < prefs.abilities.Count; i++) {
            data.abilities[i] = DecoratorHolder.ConstructAbilityStack((DecoratorHolder)prefs.abilities[i]);//.evt
        }
    }

    private void LoadTree() {
        data.abilityTree = prefs.tree.tree;
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
                        GameManager.instance.StartCoroutine(action.target.Stunned(action, node.Value.stun.stunLength));
                    }
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


    /// <summary>
    /// handles delays between attacks
    /// </summary>
    /// <returns></returns>
    private IEnumerator CharacterLimits() {
        while (true) {
            if (combatLimits.delayAfterAttacking > 0 && !combatLimits.ready) {
                yield return new WaitForSeconds(combatLimits.delayAfterAttacking);
                Debug.Log("ready to attack.");
                // pick attack.
                /*if (combatLimits.waitStun > 0) {
                    Debug.Log("waiting stun ou t ");
                    rt.isStunned ++;
                    yield return new WaitForSeconds(combatLimits.waitStun);
                    rt.isStunned --;
                }*/
                //combatLimits.waitStun = 0;
                combatLimits.ready = true;

            } else yield return null;
        }
    }
    #region STATUSES
    public void Damaged(CombatAction a, int value) {
        a.target.rt.combatUiMsg = ("-" + value+" "+Time.time);
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