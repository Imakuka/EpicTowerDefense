using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameDevHQ.TowerDefense
{
    public class TowerManager : MonoSingleton<TowerManager>
    {

        public static event Action<bool> onTurretSearching;
        public static event Action<int> onTurretCost;


        [SerializeField]
        private Camera _mainCamera;
        [Header("Decoy Turrets")]
        [SerializeField]
        private GameObject[] _decoyTurrets;//Gatling Gun = 0, Dual Gatling Gun = 1, Missle Launcher = 2, Dual AdvancedMissile Launcher = 3
        [SerializeField]
        private GameObject[] _decoyAttackRadius;//Gatling Gun = 0, Dual Gatling Gun = 1, Missle Launcher = 2, Dual AdvancedMissile Launcher = 3
        [SerializeField]
        private GameObject _currentDecoy;
        [SerializeField]
        private GameObject _currentDecoyRadius;
   
        [Header("Real Turrets")]
        [SerializeField]
        private Turret[] _weaponTurrets; //Gatling Gun = 0, Dual Gatling Gun = 1, Missle Launcher = 2, Dual AdvancedMissile Launcher = 3
        [SerializeField]
        private Turret _currentTurret;
        [SerializeField]
        private GameObject _turretTypePlaced;
        private int _turretNumberPlaced;
        private int _currentTurretType;
        private int _turretCost;
        private Vector3 _placeTurretHere;       

        [Header("Searching & Placement")]
        [SerializeField]
        private bool _turretIsSearching = false;
        [SerializeField]
        private bool _canPlaceTurret = false;
        [SerializeField]
        private MeshRenderer _currentRadiusRenderer;
        [SerializeField]
        private Color _greenColor;
        [SerializeField]
        private Color _redColor;
        private int _warFunds;
        [SerializeField]
        private GameObject _turretStorage;

        void OnEnable()
        {
            TowerSpots.onOpenTowerSpot += TowerSpotOpen;
            TowerSpots.onAttemptedPlacement += AttemptedTurretPlacement;       
        }

        void Update()
        {
            TowerPlacement();
            SearchModeOff();
        }

        void TurretSearchingMode(bool searchMode)
        {
            _turretIsSearching = searchMode;

            if (onTurretSearching != null)
            {
                onTurretSearching(_turretIsSearching);
            }

            if (searchMode == false)
            {
                _currentDecoy.SetActive(false);
                _currentDecoyRadius.SetActive(false);
            }
            
        }
        void TurretDecoyMode(int typeDecoy)
        {
            _currentDecoy = _decoyTurrets[typeDecoy];
            _currentDecoy.SetActive(true);
        }

        void ActivateDecoyRadius(int type)
        {
            _currentDecoyRadius = _decoyAttackRadius[type];
            _currentRadiusRenderer = _decoyAttackRadius[type].GetComponent<MeshRenderer>();
            if (_currentRadiusRenderer != null)
            {
                _currentDecoyRadius.SetActive(true);
            }
            
        }
        void TowerSpotOpen(bool locationOpen)
        {
            _canPlaceTurret = locationOpen;
        }
        void TurretPlacingMode(int typeTurret)
        {
            _currentTurretType = typeTurret;
            _currentTurret = _weaponTurrets[typeTurret];
        }

        void OnTurretCost(int costOfTurret)
        {
            if (onTurretCost != null)
            {
                onTurretCost(costOfTurret);
            }
        }
        void AttemptedTurretPlacement()// when mouse left click attempt to place turret
        {       
            _turretCost = _weaponTurrets[_currentTurretType].warFundsCost;

            _warFunds = GameManager.Instance.WarFunds();
            if (_warFunds >= _turretCost)
            {
                if (Input.GetMouseButtonDown(0) && _canPlaceTurret == true)// can place turret
                {
                     
                    GameObject newTurret = Instantiate(_currentTurret.turretPrefab, _placeTurretHere, Quaternion.identity);
                    newTurret.transform.parent = _turretStorage.transform;
                    _turretNumberPlaced = _currentTurret.turretType;
                    OnTurretCost(_turretCost);
                    TurretSearchingMode(false);
                    TowerSpotOpen(false);

                    if (_turretNumberPlaced == _weaponTurrets[1].turretType || _turretNumberPlaced == _weaponTurrets[3].turretType)
                    {
                        UIManager.Instance.TurretUpgradeButtons(false);
                    }
                    if (_turretNumberPlaced == _weaponTurrets[0].turretType || _turretNumberPlaced == _weaponTurrets[2].turretType)
                    {
                        UIManager.Instance.UpgradeButton();
                    }

                }

            }

        }

        public int TurretNumber()
        {
            return _turretNumberPlaced;
        }

        public void SearchModeOff()
        {
            if (Input.GetMouseButtonDown(1))
            {
                TurretSearchingMode(false);
                UIManager.Instance.UpgradeButton();
            }
        }
        public void GatlingGun() //Called with Button Clicks
        {
            TurretSearchingMode(true);
            TurretDecoyMode(0);
            TurretPlacingMode(0);
            ActivateDecoyRadius(0);         
        }
        public void MissileLauncher()// Called With Button Clicks
        {
            TurretSearchingMode(true);
            TurretDecoyMode(2);
            TurretPlacingMode(2);
            ActivateDecoyRadius(2);  
        }

        public void DualGatlingGun()// Called With Button Clicks
        {
            TurretSearchingMode(true);
            TurretDecoyMode(1);
            TurretPlacingMode(1);
            ActivateDecoyRadius(1);
        }

        public void DualMissileLauncher()// Called With Button Clicks
        {
            TurretSearchingMode(true);
            TurretDecoyMode(3);
            TurretPlacingMode(3);
            ActivateDecoyRadius(3);
        }


        void TowerPlacement()
        {
            if (_turretIsSearching == true)
            {
                Ray rayOrigin = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                
                if (Physics.Raycast(rayOrigin, out hitInfo))
                {
                    Debug.Log(hitInfo.collider.gameObject.name);
                    if (_canPlaceTurret == true)
                    {
                        _placeTurretHere = hitInfo.transform.position;
                        _currentRadiusRenderer.material.color = _greenColor;
                        _currentDecoy.transform.position = hitInfo.transform.position;
                        _currentDecoyRadius.transform.position = hitInfo.transform.position;                       
                    }
                    else
                    {
                        _currentDecoy.transform.position = hitInfo.point;
                        _currentDecoyRadius.transform.position = hitInfo.point;
                        _currentRadiusRenderer.material.color = _redColor;
                    }

                }
            }
            

        }

        void OnDisable()
        {
            TowerSpots.onOpenTowerSpot -= TowerSpotOpen;
            TowerSpots.onAttemptedPlacement -= AttemptedTurretPlacement;
        }



    }

}