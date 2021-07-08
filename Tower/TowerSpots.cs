using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameDevHQ.TowerDefense
{ 
    public class TowerSpots : MonoBehaviour
    {
        public static event Action<bool> onOpenTowerSpot;
        public static event Action onAttemptedPlacement;

        [SerializeField]
        private GameObject _towerLocation;
        [SerializeField]
        private GameObject _turretType;
        [SerializeField]
        private int _currentTurretNumber;     
        [SerializeField]
        private GameObject _particleBeam;
        [SerializeField]
        private ParticleSystem _particle;
        [SerializeField]
        private bool _turretSearching = false;
        [SerializeField]
        private bool _isTowerAvailable = true;
        [SerializeField]
        private bool _upgradeMode = false;


        private void OnEnable()
        {
            TowerManager.onTurretSearching += OnTurretSearchMode;
            UIManager.onTurretUpgradeMode += TurretUpgrade;
            UIManager.onTurretSale += OnTurretSale;
            TurretControl.onTurretDestroy += OnTurretDestroy;
            _particle = _particleBeam.GetComponent<ParticleSystem>();
            if(_particle == null)
            {
                Debug.LogError("TowerSpots:: OnEnable: The particle System is Null");
            }
        }


        public void OnTurretDestroy(GameObject turret)
        {
            if (_turretType == turret)
            {
                _isTowerAvailable = true;
            }
        }

        void OnTurretSale(int profit, GameObject location)
        {
            if (_towerLocation == location)
            {
                _turretType.SetActive(false);
                _isTowerAvailable = true;
            }
        }

        public void TurretUpgrade( bool upgradeMode)
        {
            _upgradeMode = upgradeMode;

            if (upgradeMode == true && _isTowerAvailable == false)
            {
                if (_currentTurretNumber <= 2)
                {
                    _particleBeam.transform.localScale = new Vector3(.4f, .4f, .4f);
                    var newColor = _particle.main;
                    newColor.startColor = Color.blue;
                    _particleBeam.SetActive(true);
                }

            }
            else
            {
                _particleBeam.SetActive(false);
                var particleColor = _particle.main;
                particleColor.startColor = Color.green;
            }
        }
        void OnTurretSearchMode(bool searching)
        {
            _turretSearching = searching;

            if (searching == true && _isTowerAvailable == true)
            {
                _particleBeam.SetActive(true);
            }
            if (searching == false)
            {
                _particleBeam.SetActive(false);
            }

        }

        void OnOpenTowerSpot(bool spotOpen)
        {
            if (onOpenTowerSpot != null)
            {
                onOpenTowerSpot(spotOpen);
            }
        }

        void OnMouseEnter()
        {
            if(_turretSearching == true && _isTowerAvailable == true)
            {
                OnOpenTowerSpot(true);
            }
            
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "TowerGun")
            {
                _turretType = other.gameObject;
            }
        }

        void OnMouseDown()
        {

            if (_turretSearching == true && _isTowerAvailable)
            {
                if (onAttemptedPlacement != null)
                {
                    onAttemptedPlacement();
                    _isTowerAvailable = _turretSearching;
                    _currentTurretNumber = TowerManager.Instance.TurretNumber();//Grabs the Turret Type Number from the Scriptable Obj


                    if (_turretSearching == false && _isTowerAvailable == false)
                    {
                        _particleBeam.SetActive(false);
                    }
    
                }
            }
            else if (_upgradeMode == true)
            {
                if (_currentTurretNumber == 0 || _currentTurretNumber == 2)
                {
                    UIManager.Instance.UpgradingTurret(_towerLocation, _currentTurretNumber);
                }

            }
        }

        void OnMouseExit()
        {
            OnOpenTowerSpot(false);
        }

        

        void OnDisable()
        {
            TowerManager.onTurretSearching -= OnTurretSearchMode;
            UIManager.onTurretUpgradeMode -= TurretUpgrade;
            UIManager.onTurretSale -= OnTurretSale;
            TurretControl.onTurretDestroy -= OnTurretDestroy;
        }
    }
}
