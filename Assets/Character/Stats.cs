using UnityEngine;

namespace Fantasy
{
    public class Stats : MonoBehaviour
    {
        public enum CharStatus : int
        {
            Health,
            Vitality,
            Mana,
            Stamina,
            Endurance,
            Weight,
            Agility,
            Patk,
            Strength,
            Pdef,
            Matk,
            Inteligence,
            Mdef,
            Luck,
            Faith,
        }

        public enum DeBuff
        {
            Burn,
            Poison,
            Regen,
            Stun,
            Sleep,
            Freeze,
            Blind,
            Weaken,
            PatkModify,
            PdefModify,
            MatkModify,
            MdefModify,
            SpeedModify,
            Fear,
        }

        public float[] stats;

        public bool Damage(float damage)
        {
            stats[(int)CharStatus.Health] -= damage;
            return true;
        }
    }
}
