using UnityEngine;

namespace Fantasy
{
    [CreateAssetMenu(fileName = "Spell", menuName = "Combat/Spell", order = 3)]
    public class Spell : Action
    {
        public Elements[] incantation;

        public enum Elements : int
        {
            EndStart = -2,
            None = -1,
            Fire = 0,
            Water = 1,
            Earth = 2,
            Wind = 3,
            Light = 4,
            Dark = 5,
        }

        public override void Execute(Animator anim)
        {
            anim.Play(animation.name);
        }
    }
}
