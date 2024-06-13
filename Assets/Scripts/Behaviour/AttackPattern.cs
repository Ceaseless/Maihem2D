using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maihem
{
    public abstract class AttackPattern : ScriptableObject
    {
        [SerializeField] private int damage;

        public int Damage => damage;
        public abstract Vector2Int Attack(Vector2Int position);
    }
}
