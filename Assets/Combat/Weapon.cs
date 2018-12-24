using UnityEngine;

namespace Fantasy
{
    public class Weapon : Damager
    {
        protected override void OnHit(Collider other, HitMessage msg)
        {
            // TODO: hit effects
        }
        protected override void OnHitDamage(Stats other)
        {
            // TODO: damage effects
        }
        protected override float GetScalledStat()
        {
            return caster.GetComponentInParent<Stats>().stats[(int)Stats.CharStatus.Strength];
        }
    }
}
