using UnityEngine;

namespace Fantasy
{
    public abstract class Action : ScriptableObject
    {
        [System.Flags]
        public enum ActionType
        {
            NoDamage = 0x00001,
            Melee = 0x00010,
            Range = 0x00100,
            AOE = 0x01000,
            Target = 0x10000,
            MeleeRange = Melee | Range,
        }

        public AnimationClip animation;
        public int dmg;
        public ActionType type;
        public GameObject attackSpawn;

        public abstract void Execute(Animator anim);
    }
}
