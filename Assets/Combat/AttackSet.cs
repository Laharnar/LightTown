using System.Collections.Generic;
using UnityEngine;

namespace Fantasy
{
    [CreateAssetMenu(fileName = "AttackSet", menuName = "Combat/AttackSet", order = 2)]
    public class AttackSet : ScriptableObject
    {
        public List<Attack> attacks = new List<Attack>();
    }
}
