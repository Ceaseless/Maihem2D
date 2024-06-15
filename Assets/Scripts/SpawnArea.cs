using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maihem
{
    public abstract class SpawnArea : MonoBehaviour
    {
        [SerializeField] private int size = 1;
        [SerializeField] private GameObject[] possibleSpawns;

        public virtual GameObject GetSpawnableGameObject()
        {
            return possibleSpawns[Random.Range(0, possibleSpawns.Length)];
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * size); 
        }
    }
}