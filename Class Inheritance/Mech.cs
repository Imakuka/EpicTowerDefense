using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameDevHQ.TowerDefense
{
    [System.Serializable]
    public abstract class Mech : MonoBehaviour
    {
        [Header("Mech Details")]
        [SerializeField]
        protected string _itemID;
        [SerializeField]
        protected int _warProfits;

        [Header("Health")] 
        [SerializeField]
        protected int _health;
        [Range(0, 100)]
        [SerializeField]
        protected int _currentHealth;
        [SerializeField]
        protected int _damageToTake;
        [SerializeField]
        protected Renderer _healthRend;
    }
}


