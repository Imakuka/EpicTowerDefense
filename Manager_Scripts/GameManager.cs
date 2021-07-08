using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GameDevHQ.TowerDefense
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Header("War Funds")]
        [SerializeField]
        private int _warFunds;
        private int _turretCost;
        [Header("Components")]
        [SerializeField]
        private Transform _finish;
        [SerializeField]
        private Transform _start;
        [SerializeField]
        private Transform _missileHangar;
        [Header("Player Health")]
        [SerializeField]
        private int _playerHealth;
        [SerializeField]
        private int _playerLives = 5;

        private WaitForSeconds _restartYield;
        private float _restartOffset = 2.0f;

        

        void Start()
        {
            UIManager.Instance.UpdateWarFundsAmount(_warFunds);
            UIManager.Instance.UpdatePlayerLivesCount(_playerLives);
            TowerManager.onTurretCost += TurretCost;
            UIManager.onTurretSale += OnTurretSale;
            _playerHealth = 100;
            StartCoroutine(UIManager.Instance.BeginingOfGameRoutine());
            _restartYield = new WaitForSeconds(_restartOffset);

        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);//Main Menu
            }
        }

        public void StartGameButton()
        {
            SceneManager.LoadScene(1);//Main Game
        }

        public void RestartMode()
        {
            UIManager.Instance.Restart(true);
            StartCoroutine(RestartRoutine());
        }

        IEnumerator RestartRoutine()
        {
            yield return _restartYield;
            SceneManager.LoadScene(1);//Main Game
            SpawnManager.Instance.CanSpawn(false);
            UIManager.Instance.Restart(false);
        }

        public void PlayModeOn()
        {
            Time.timeScale = 1;
            UIManager.Instance.PlayMode(true);
        }
        public void PauseGame()
        {
            Time.timeScale = 0;
            UIManager.Instance.PauseMode(true);
        }
        public void FFMode()
        {
            Time.timeScale = 2;
            UIManager.Instance.FastForwardMode(true);
        }
        public void PlayerHealth(int damage)
        {
            int playerHealth = _playerHealth - damage;
            _playerHealth = playerHealth;

            if (_playerHealth < 70 && _playerHealth > 40)
            {
                UIManager.Instance.ChangeUIColor(1);
            }
            if (_playerHealth < 40)
            {
                UIManager.Instance.ChangeUIColor(2);
            }
            if (_playerHealth <= 0)
            {
                _playerHealth = 0;
                _playerLives--;
                UIManager.Instance.UpdatePlayerLivesCount(_playerLives);
                
                if (_playerLives <= 0)
                {
                    _playerLives = 0;
                    RestartMode();          
                }
                else
                {
                    UIManager.Instance.PauseMode(true);
                    UIManager.Instance.LostALife();
                    _playerHealth = 100;
                    UIManager.Instance.ChangeUIColor(0);
                }
            }

        }

        void OnTurretSale(int profit, GameObject location)
        {
            _warFunds += profit;
            UIManager.Instance.UpdateWarFundsAmount(_warFunds);
        }

        public void OnMechDeath(int warFunds)
        {
            _warFunds += warFunds;
            UIManager.Instance.UpdateWarFundsAmount(_warFunds);
        }

        void TurretCost(int costOfTurret)
        {
            _turretCost = costOfTurret;
            if (costOfTurret > 0)
            {
                _warFunds -= _turretCost;

                UIManager.Instance.UpdateWarFundsAmount(_warFunds);
            }
        }
        public int WarFunds()
        {
            return _warFunds;
        }

        public Transform GetMissileHangar()
        {
            return _missileHangar;
        }

        public Transform GetFinish()
        {
            return _finish;
        }

        public Transform GetStart()
        {
            return _start;
        }

        void OnDisable()
        {
            TowerManager.onTurretCost -= TurretCost;
            UIManager.onTurretSale -= OnTurretSale;
        }


    }
}

