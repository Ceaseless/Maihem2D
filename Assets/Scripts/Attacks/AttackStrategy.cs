using System.Collections.Generic;
using UnityEngine;

namespace Maihem.Attacks
{

    public abstract class AttackStrategy : ScriptableObject
    {
        [Min(0)]
        [SerializeField] private int damage;
        [Min(0)]
        [SerializeField] private int staminaCost;

        public int Damage => damage;
        public int StaminaCost => staminaCost;

        public abstract void Attack(Vector2Int position, Vector2Int direction);
        public abstract IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction);

    }
}
