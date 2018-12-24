using UnityEngine;

namespace Fantasy
{
    public class Projectile : Damager
    {
        protected override void OnHitDamage(Stats other)
        {
            // TODO: spawn effect
        }
        protected override void OnHit(Collider other, HitMessage msg)
        {
            // TODO: Destroy effect
            switch (msg)
            {
                case HitMessage.None:
                    break;
                case HitMessage.Damage:
                    Destroy(gameObject);
                    break;
                case HitMessage.Destroy:
                    Destroy(gameObject);
                    break;
            }
        }
        protected override float GetScalledStat()
        {
            return caster.GetComponentInParent<Stats>().stats[(int)Stats.CharStatus.Inteligence];
        }
    }
}
