using UnityEngine;

namespace Fantasy
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Combat/Attack", order = 1)]
    public class Attack : Action
    {
        public float precastTime = 0.5f;

        public override void Execute(Animator anim)
        {
            anim.Play(animation.name);
        }
    }
}
