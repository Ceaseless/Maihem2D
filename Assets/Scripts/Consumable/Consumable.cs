using System.Collections;
using System.Collections.Generic;
using Maihem.Actors;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public abstract class Consumable : MonoBehaviour
    {
        [SerializeField] public Sprite sprite;

        public void Activate()
        {
            OnActivation();
        }

        protected abstract void OnActivation();
    }
}
