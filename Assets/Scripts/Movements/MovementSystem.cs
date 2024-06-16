using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Maihem.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Maihem
{
    public class MovementSystem : MonoBehaviour
    {
        [SerializeField] private MovementStrategy currentStrategy;

        private Random _randomNumbers;

        private bool _isActivated;

        private void Awake()
        {
            _isActivated = false;
            _randomNumbers = new Random();
        }

        private void CheckAlert( Vector2Int gridPosition)
        {
            if (_isActivated == false)
            {
                _isActivated = currentStrategy.CheckAlert(gridPosition);
            }
        }

        public Vector2Int Move(Vector2Int gridPosition)
        {
            CheckAlert(gridPosition);
            return _isActivated ? currentStrategy.ActivatedMove(gridPosition) : currentStrategy.IdleMove(gridPosition,_randomNumbers);
        }
    }
}
