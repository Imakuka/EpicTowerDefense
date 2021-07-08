using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace GameDevHQ.TowerDefense
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [Header("Health Bar Pool")]
        [SerializeField]
        private GameObject _mechHealthBarPrefab;
        [SerializeField]
        private GameObject _healthBarStorage;
        [SerializeField]
        private int _healthBarAmount = 20;
        [SerializeField]
        private List<GameObject> _healthBars = new List<GameObject>();

        [Header("Missile Pool")]
        [SerializeField]
        private GameObject _missilePrefab;
        [SerializeField]
        private GameObject _missileHangar;
        [SerializeField]
        private int _poolAmount = 20;
        [SerializeField]
        private List<GameObject> _missiles = new List<GameObject>();

        [Header("Mech Pool")]
        [SerializeField] 
        private GameObject[] _mechTypes;
        [SerializeField]
        private GameObject _mechGarage;
        [SerializeField]
        private int _poolSize = 10;
        [SerializeField]
        private Dictionary<int, List<GameObject>> enemyDictionary = new Dictionary<int, List<GameObject>>();

        // Start is called before the first frame update
        void Start()
        {
            CreateMechPool();
            CreateMissilePool();
            CreateHealthBarPool();
        }

        void CreateHealthBarPool()
        {
            for (int i = 0; i < _healthBarAmount; i++)
            {
                GameObject newHealthbar = Instantiate(_mechHealthBarPrefab);
                newHealthbar.transform.parent = _healthBarStorage.transform;
                newHealthbar.SetActive(false);
                _healthBars.Add(newHealthbar);
            }
        }

        void CreateMissilePool()
        {
            for (int i = 0; i < _poolAmount; i++)
            {
                GameObject newMissile = Instantiate(_missilePrefab);
                newMissile.transform.parent = _missileHangar.transform;
                newMissile.SetActive(false);
                _missiles.Add(newMissile);
            }
        }

        void CreateMechPool()
        {
            for (var i = 0; i < _mechTypes.Length; i++)
            {
                List<GameObject> listToAdd = new List<GameObject>();
                for(var j = 0; j < _poolSize; j++)
                {
                    GameObject newEnemy = Instantiate(_mechTypes[i]);
                    newEnemy.transform.parent = _mechGarage.transform;
                    newEnemy.SetActive(false);
                    listToAdd.Add(newEnemy);        
                }
                enemyDictionary.Add(i, listToAdd);
            }

        }

        public Transform MissileHangar()
        {
            return _missileHangar.transform;
        }

        public GameObject MissileRequest()
        {
            var missile = _missiles.FirstOrDefault((m) => m.activeInHierarchy == false);

            if (missile != null)
            {
                return missile;
            }

            return null;

        }

        public GameObject EnemyRequest(int type)
        {
            var enemy = enemyDictionary[type].FirstOrDefault((e) => e.activeInHierarchy == false);

            if (enemy != null)
            {
                return enemy;
            }

            return null;
        }
        public int ReturnEnemyTypeLength()
        {
            int enemyLengthType = _mechTypes.Length;
            return enemyLengthType;
        }


    }
}


