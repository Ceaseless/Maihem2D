using System;
using System.Collections;
using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public class TutorialMap : MonoBehaviour
    {
        private void Awake()
        {
            GameManager.Instance.ToggleKillZone(true);
            GameManager.Instance.TogglePlayerStats(false);
        }
        
    }
}
