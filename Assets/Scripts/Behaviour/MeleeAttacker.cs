using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    [CreateAssetMenu(menuName = "Attack Pattern/Melee Attacker")]
    public class MeleeAttacker : AttackPattern
    {
        public override Vector2Int Attack(Vector2Int position)
        {
            var player = GameManager.Instance.Player;
            var neighbours = MapManager.GetNeighbourPositions(position);
            if (!neighbours.Contains(player.GridPosition))
            {
                return new Vector2Int(0,0);
            }
            
            player.TakeDamage(Damage);

            return player.GridPosition - position;
        }
    }
}
