using UnityEngine;

namespace Fantasy
{
    public class PlayerSpellCombatControler : SpellCombat
    {
        public override Spell.Elements ReceiveSkillCommands()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                return Spell.Elements.Fire;
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                return Spell.Elements.Water;
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                return Spell.Elements.Earth;
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                return Spell.Elements.Wind;
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                return Spell.Elements.EndStart;
            }
            return Spell.Elements.None;
        }
    }
}