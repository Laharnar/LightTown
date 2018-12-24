using UnityEngine;

namespace Fantasy
{
    public abstract class Damager : MonoBehaviour
    {
        public enum HitMessage
        {
            None = 0,
            Damage = 1,
            Destroy = 2,
        }

        public float atk;
        public float strIntScl;
        internal Action UsedAction;
        internal MonoBehaviour caster;

        private void OnTriggerEnter(Collider other)
        {
            // damage other
            HitMessage hmsg = GenerateHitMessage(other, out object obj);
            switch (hmsg)
            {
                case HitMessage.None:
                    break;
                case HitMessage.Damage:
                    ((Stats)obj).Damage(UsedAction.dmg + atk + strIntScl * GetScalledStat());
                    OnHitDamage((Stats)obj);
                    break;
                case HitMessage.Destroy:
                    break;
            }
            OnHit(other, hmsg);
        }

        /// <summary>
        /// Generate a message depending on the data of the object that is hit
        /// </summary>
        /// <param name="other">The collider that was collided with</param>
        /// <param name="obj">Output data that is useful for responding on the gotten message</param>
        /// <returns></returns>
        protected virtual HitMessage GenerateHitMessage(Collider other, out object obj)
        {
            Stats stats = other.GetComponentInParent<Stats>();
            if (caster != null && caster.GetComponent<Stats>() != stats)
            {
                if (stats != null)
                {
                    obj = stats;
                    return HitMessage.Damage;
                }
                else
                {
                    obj = other;
                    return HitMessage.Destroy;
                }
            }
            obj = null;
            return HitMessage.None;
        }
        protected abstract void OnHitDamage(Stats other);
        protected abstract void OnHit(Collider other, HitMessage msg);
        protected abstract float GetScalledStat();
    }
}
