using UnityEngine;
using System.Collections;

namespace Fantasy
{
    public abstract class MeleeCombat : MonoBehaviour
    {
        private Animator anim;
        private CharacterState charState;

        public enum AttackSetID : int
        {
            None = -1,
            Normal = 0,
            Special = 1,
            Dash = 2,
        }
        [System.Serializable]
        public struct AttackID
        {
            public AttackSetID atkSetID;
            public int atkID;

            public AttackID(AttackSetID atkSetID, int atkID)
            {
                this.atkSetID = atkSetID;
                this.atkID = atkID;
            }

            public bool IsNull()
            {
                return atkSetID == AttackSetID.None && atkID == -1;
            }
            public static AttackID Null
            {
                get { return new AttackID(AttackSetID.None, -1); }
            }
        }

        public AttackSet[] attackSets = new AttackSet[3];
        public AttackID curAtk = AttackID.Null;
        public AttackID nextAttack = AttackID.Null;

        private void Start()
        {
            anim = GetComponent<Animator>();
            charState = GetComponent<CharacterState>();
        }

        private void Update()
        {
            if (!charState.isAttacking)
            {
                // check for attack command
                if (nextAttack.IsNull())
                {
                    AttackSetID atkOrder = ReceiveAtkCommands();
                    if (atkOrder != AttackSetID.None)
                    {
                        Attack(new AttackID(atkOrder, 0));
                    }
                }
                // execute next attack
                else
                {
                    Attack(nextAttack);
                    nextAttack = AttackID.Null;
                }
                return;
            }

            // check for command for next attack
            AttackSetID atkOrder2 = ReceiveAtkCommands();
            if (atkOrder2 != AttackSetID.None)
            {
                AttackSet atkSet = attackSets[(int)curAtk.atkSetID];
                Attack atk = atkSet.attacks[curAtk.atkID];
                float atkLength = atk.animation.length;
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime * atkLength >= atkLength - atk.precastTime)
                {
                    // execute next attack in set
                    if (atkOrder2 == curAtk.atkSetID && curAtk.atkID + 1 < atkSet.attacks.Count)
                    {
                        nextAttack = new AttackID(atkOrder2, curAtk.atkID + 1);
                    }
                    // break set and execute first attack of new set or repeat set
                    else
                    {
                        nextAttack = new AttackID(atkOrder2, 0);
                    }
                }
            }
        }

        public abstract AttackSetID ReceiveAtkCommands();

        protected void Attack(AttackID atkID)
        {
            if (atkID.IsNull())
            {
                return;
            }
            Attack atk = attackSets[(int)atkID.atkSetID]?.attacks[atkID.atkID];
            if (atk != null)
            {
                curAtk = atkID;
                Weapon[] weapons = GetComponentsInChildren<Weapon>();
                for (int i = 0; i < weapons.Length; i++)
                {
                    weapons[i].UsedAction = atk;
                    weapons[i].caster = this;
                }
                atk.Execute(anim);
            }
        }
    }
}
