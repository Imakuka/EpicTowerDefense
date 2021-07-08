using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameDevHQ.TowerDefense
{

    [CreateAssetMenu(menuName = "Scriptable Obj/New Turret")]
    public class Turret : ScriptableObject
    {
        public int turretType;
        public int warFundsCost;
        public GameObject turretPrefab;


    }
}

