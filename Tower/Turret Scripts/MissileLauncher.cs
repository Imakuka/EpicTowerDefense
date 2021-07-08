using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameDevHQ.TowerDefense
{
    public class MissileLauncher : TurretControl
    {
        public static event Action<Transform, float, float> onAssignMissileRules;

        [Header("Missile Launcher")]
        [SerializeField]
        private GameObject[] _missilePositions;
        [SerializeField]
        private float _reloadTime; //time in between reloading the rockets
        [Header("Missiles Controls")]
        [SerializeField]
        private float _fireDelay; //fire delay between rocketst
        [SerializeField]
        private float _power; //power to apply to the force of the rocket
        [SerializeField]
        private float _destroyTime = 4.0f; //how long till the rockets get cleaned up

        private WaitForSeconds _fireYield;
        private WaitForSeconds _reloadYield;

        void Start()
        {
            _fireYield = new WaitForSeconds(_fireDelay);
            _reloadYield = new WaitForSeconds(_reloadTime);
        }

        public void OnAssignMissileRules()
        {
            if (onAssignMissileRules != null)
            {
                onAssignMissileRules(currentTarget.transform, _power, _destroyTime);
            }
        }
        protected override void FireWeapon(bool canFire)
        {
            base.FireWeapon(canFire);
            if (canFire == true)
            {
                StartCoroutine(FireRocketsRoutine());
            }

        }

        IEnumerator FireRocketsRoutine()
        {

            for (int i = 0; i < _missilePositions.Length; i++) //for loop to iterate through each missle position
            {
                if (currentTarget != null)
                {
                    GameObject rocket = PoolManager.Instance.MissileRequest();
                    rocket.transform.position = _missilePositions[i].transform.position; //set the rockets parent to the missle launch position 
                    rocket.SetActive(true);
                    rocket.transform.localEulerAngles = transform.localEulerAngles;  
                    OnAssignMissileRules();
                    _missilePositions[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired

                    if (i >= _missilePositions.Length - 1)
                    {
                        i = 0;
                    }
                    yield return _fireYield; //how long between each rocket

                }

            }

            for (int j = 0; j < _missilePositions.Length; j++) //itterate through missle positions
            {
                yield return _reloadYield; //wait for reload time
                _missilePositions[j].SetActive(true); //enable fake rocket to show ready to fire
            }
        }

    }
}


