using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameDevHQ.TowerDefense
{
    public class SpawnManager : MonoSingleton<SpawnManager>
    {

        //[SerializeField]
        public int _waveNumber;
        [SerializeField]
        private int _currentWaveAmount;
        [SerializeField]
        private int _amountSpawned = 0;
        [SerializeField]
        private int _enemiesDestroyed;
        [SerializeField]
        private Wave[] _theWaves;
        [SerializeField]
        private Transform _start;//spawn manager is the start
        [SerializeField]
        private bool _canSpawn = false;

        private WaitForSeconds _spawnYield;
        [SerializeField]
        private float _spawnDelay = 8.0f;
        private WaitForSeconds _messageDisplayYield;
        private float _displayTime = 5.0f;
        private WaitForSeconds _startNextWaveYield;
        private float _nextWaveOffset;

        // Start is called before the first frame update
        void Start()
        {
            DefineComponents();
        }

        public Wave TheWaves(int waveNumber)
        {
            
            return _theWaves[waveNumber];
        }
        void DefineComponents()
        {
            _start = transform;
            if (_start == null)
            {
                Debug.Log("The Start is null");
            }
            UIManager.Instance.UpdateWavesCount(_waveNumber);
            _spawnYield = new WaitForSeconds(_spawnDelay);
            _messageDisplayYield = new WaitForSeconds(_displayTime);
            _startNextWaveYield = new WaitForSeconds(_nextWaveOffset);
        }

        public void CanSpawn(bool spawning)
        {
            _canSpawn = spawning;

            StartCoroutine(WaveSpawnRoutine());
        }
        private IEnumerator WaveSpawnRoutine()
        {
            _currentWaveAmount = _theWaves[_waveNumber].amountToSpawn;

            while (_canSpawn == true)
            {
                for (int i = 0; i < _currentWaveAmount; i++)
                {
                    
                    int enemyType = _theWaves[_waveNumber].ReturnEnemyType(i);
                    GameObject newEnemy = PoolManager.Instance.EnemyRequest(enemyType);
                    newEnemy.SetActive(true);
                    _amountSpawned++;
                    yield return _spawnYield;

                    /*if (newEnemy.name == "Mech1")
                    {
                        yield return new WaitForSeconds(5);
                        Debug.Log("Spawned time: " + Time.realtimeSinceStartup);
                    }
                    else
                    {
                        yield return new WaitForSeconds(10);
                        Debug.Log("Spawned time: " + Time.realtimeSinceStartup);
                    }*/

                    if (_amountSpawned == _currentWaveAmount)
                    {
                        _canSpawn = false;
                    }
                }
            }




        }

        private IEnumerator NextWaveRoutine()
        {
            yield return _messageDisplayYield;//Display Messae Time
            UIManager.Instance.WaveComplete(false);
            yield return _startNextWaveYield;//Time for player to build or upgrade

            UIManager.Instance.StartLevel(); 
        }
       
        public void WaveComplete()
        {
            _enemiesDestroyed++;
            if (_enemiesDestroyed == _currentWaveAmount)
            {
                UIManager.Instance.WaveComplete(true);

                _enemiesDestroyed = 0;
                _waveNumber++;
                UIManager.Instance.UpdateWavesCount(_waveNumber);
                StartCoroutine(NextWaveRoutine());
            }
        }




    }
}


