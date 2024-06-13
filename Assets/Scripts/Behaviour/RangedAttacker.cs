using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    [CreateAssetMenu(menuName = "Attack Pattern/Ranged Attacker")]
    public class RangedAttacker : AttackPattern
    {
        [SerializeField] private int range;
        public override Vector2Int Attack(Vector2Int position)
        {
            var player = GameManager.Instance.Player;
            List<Vector2Int> lineToPlayer = MapManager.Instance.IsInDirectLine(position, player.GridPosition, range);
            if (!lineToPlayer.Contains(player.GridPosition))
            {
                return new Vector2Int(0,0);
            }
            
            Debug.Log("Enemy Ranged Attack");
            player.TakeDamage(Damage);

            return player.GridPosition - position;
        }
    }
}
