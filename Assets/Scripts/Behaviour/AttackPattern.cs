using UnityEngine;

namespace Maihem.Behaviour
{
    public abstract class AttackPattern : ScriptableObject
    {
        [SerializeField] private int damage;

        public int Damage => damage;
        public abstract Vector2Int Attack(Vector2Int position);
    }
}
