using System;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Extensions
{
    public static class FacingExtensions
    {
        public static Vector2Int GetFacingVector(this Facing facing)
        {
            return facing switch
            {
                Facing.East => Vector2Int.right,
                Facing.West => Vector2Int.left,
                Facing.North => Vector2Int.up,
                Facing.South => Vector2Int.down,
                Facing.NorthEast => new Vector2Int(1,1),
                Facing.SouthEast => new Vector2Int(1,-1),
                Facing.SouthWest => new Vector2Int(-1,-1),
                Facing.NorthWest => new Vector2Int(-1,1),
                _ => throw new ArgumentOutOfRangeException(nameof(facing), facing, null)
            };
        }

        public static Facing GetFacingFromDirection(this Facing facing, Vector2Int direction)
        {
            return (math.clamp(direction.x,-1,1), math.clamp(direction.y,-1,1)) switch
            {
                (1, 0) => Facing.East,
                (-1, 0) => Facing.West,
                (0, 1) => Facing.North,
                (0, -1) => Facing.South,
                (1,1) => Facing.NorthEast,
                (1,-1) => Facing.SouthEast,
                (-1,-1) => Facing.SouthWest,
                (-1,1) => Facing.NorthWest,
                (0,0) => facing,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        // E -> SE
        // SE -> S
        // S -> SW
        // SW -> W
        // W -> NW
        // NW -> N
        // N -> NE
        // NE -> E
        public static Facing NextFacingClockWise(this Facing facing)
        {
            return facing switch
            {
                Facing.East => Facing.SouthEast,
                Facing.West => Facing.NorthWest,
                Facing.North => Facing.NorthEast,
                Facing.South => Facing.SouthWest,
                Facing.NorthEast => Facing.East,
                Facing.SouthEast => Facing.South,
                Facing.SouthWest => Facing.West,
                Facing.NorthWest => Facing.North,
                _ => throw new ArgumentOutOfRangeException(nameof(facing), facing, null)
            };
        }
        
        // E -> NE
        // NE -> N
        // N -> NW
        // NW -> W
        // W -> SW
        // SW -> S
        // S -> SE
        // SE -> E
        public static Facing NextFacingCounterClockWise(this Facing facing)
        {
            return facing switch
            {
                Facing.East => Facing.NorthEast,
                Facing.West => Facing.SouthWest,
                Facing.North => Facing.NorthWest,
                Facing.South => Facing.SouthEast,
                Facing.NorthEast => Facing.North,
                Facing.SouthEast => Facing.East,
                Facing.SouthWest => Facing.South,
                Facing.NorthWest => Facing.West,
                _ => throw new ArgumentOutOfRangeException(nameof(facing), facing, null)
            };
        }
       
    }
}