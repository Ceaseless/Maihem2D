using UnityEngine;

namespace Maihem
{
    [CreateAssetMenu(fileName = "Object Bound Settings")]
    public class ObjectBoundSettings : ScriptableObject
    {
        public float maxUpdateDistance;
        public float activateDistance;
        public float horizontalCullDistance;

        public bool IsInActivateDistance(Vector3 origin, Vector3 targetPosition)
        {
            return Mathf.Abs(origin.x - targetPosition.x) < activateDistance;
        }
        
        public bool IsInUpdateDistance(Vector3 origin, Vector3 targetPosition)
        {
            return Mathf.Abs(origin.x - targetPosition.x) < maxUpdateDistance;
        }

        
        public bool IsOutsideOfCullDistance(Vector3 origin, Vector3 targetPosition)
        {
            var distance = origin.x - targetPosition.x; // > 0 => Player is on the right
            return distance > horizontalCullDistance;
        }
    }
}