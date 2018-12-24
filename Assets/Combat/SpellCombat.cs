using System.Collections.Generic;
using UnityEngine;
using static Fantasy.Spell;

namespace Fantasy
{
    public abstract class SpellCombat : MonoBehaviour
    {
        public List<Spell> spells = new List<Spell>();

        public AnimationClip channelAnim;

        private Animator anim;
        private CharacterState charState;
        private List<Elements> incantation = new List<Elements>();
        private Spell curSpell;

        private void Start()
        {
            anim = GetComponent<Animator>();
            charState = GetComponent<CharacterState>();
        }

        private void Update()
        {
            Spell.Elements input = ReceiveSkillCommands();
            if (charState.isIncantating)
            {
                // cast the spell
                if (input == Spell.Elements.EndStart)
                {
                    FinishIncantation();
                }
                else if (input >= 0)
                {
                    incantation.Add(input);
                }
                return;
            }
            else if (input == Spell.Elements.EndStart)
            {
                BeginIncantation();
            }
        }

        public abstract Elements ReceiveSkillCommands();

        protected void TryCast(List<Elements> incantation)
        {
            if (incantation.Count <= 0 || incantation[0] == Elements.None)
            {
                return;
            }
            // check for spell with correct incantation
            foreach (var spell in spells)
            {
                bool cast = true;
                if (spell.incantation.Length == incantation.Count)
                {
                    for (int j = 0; j < spell.incantation.Length; j++)
                    {
                        if (spell.incantation[j] != incantation[j])
                        {
                            cast = false;
                            break;
                        }
                    }
                }
                else
                {
                    cast = false;
                }
                if (cast)
                {
                    Cast(spell);
                    return;
                }
            }
        }

        private void Cast(Spell spell)
        {
            if (spell == null || spell.incantation.Length == 0)
            {
                return;
            }
            curSpell = spell;
            spell.Execute(anim);
        }

        protected void BeginIncantation()
        {
            charState.isIncantating = true;
            anim.Play(channelAnim.name);
        }
        private void FinishIncantation()
        {
            TryCast(incantation);
            incantation.Clear();
            charState.isIncantating = false;
        }

        public void SpawnProjectilEvent()
        {
            GameObject proj = Instantiate(curSpell.attackSpawn, transform.position, transform.rotation, null);
            proj.GetComponent<Damager>().UsedAction = curSpell;
            proj.GetComponent<Damager>().caster = this;
        }
    }
}
