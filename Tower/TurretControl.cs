using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameDevHQ.TowerDefense;


namespace GameDevHQ.TowerDefense
{
    public abstract class TurretControl : MonoBehaviour
    {
        public static event Action<GameObject, int> onTargetToDamage;//Target to take damage and how much
        public static event Action<GameObject> onTurretDestroy;

        Queue<GameObject> _enemy = new Queue<GameObject>();

        [SerializeField]
        protected GameObject[] _queueTest;

        [Header("Turret Control Abstract Class")]
        [SerializeField]
        protected GameObject currentTarget;
        [SerializeField]
        protected GameObject activeTurret;
        [SerializeField]
        protected GameObject _nextTarget;
        [SerializeField]
        protected int _damageAmount;
        [SerializeField]
        protected bool _canFire;
        [SerializeField]
        protected int _turretArmor;
        [SerializeField]
        protected GameObject _explosionPrefab;

        [Header("Rotate Functions")]
        [SerializeField]
        protected Transform rotateGunBase;
        protected Vector3 directionToRotate;
        protected Quaternion targetRotation;
        [SerializeField]
        protected float rotationSpeed = 6.5f;

        private WaitForSeconds _destroyTurretYield;
        private float _destroyTurretOffset = 1.5f;
       
        
        protected virtual void OnEnable()
        {
            activeTurret = gameObject;//This GameObject
            EnemyAI.onEnemyDeath += EnemyDied;
            EnemyAI.onTurretDamage += OnTurretDamage;
            _destroyTurretYield = new WaitForSeconds(_destroyTurretOffset);
        }
    
        protected virtual void FireWeapon(bool canFire)
        {
            //This is called from the individual turrets
        }

        private void OnTurretDamage(GameObject turret, int damageToTake)
        {
            if (gameObject == turret)
            {
                _turretArmor -= damageToTake;
            }

            if (_turretArmor <= 0)
            {
                if (onTurretDestroy != null)
                {
                    onTurretDestroy(turret);
                }
                _turretArmor = 0;
                StartCoroutine(DestroyTurretRoutine());
            }
        }

        IEnumerator DestroyTurretRoutine()
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity); //instantiate explosion
            explosion.transform.parent = transform.transform;
            
            yield return _destroyTurretYield;
            gameObject.SetActive(false);
            Destroy(explosion.gameObject);
            Destroy(this.gameObject);
        }

        protected void OnTargetToDamage() 
        {
            if (onTargetToDamage != null)
            {
                onTargetToDamage(currentTarget.gameObject, _damageAmount);           
            }
        }

        void EnemyDied(GameObject whoDied)
        {
            if (currentTarget == whoDied)
            {
                _enemy.Dequeue();//can cause issue with multiple turrets
                FireWeapon(false);
                currentTarget = null;
                TryNextTarget();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {             
                _enemy.Enqueue(other.gameObject);
                if (currentTarget == null)//if I dont have an enemy
                {
                    currentTarget = other.gameObject;
                    FireWeapon(true);
                }              
            }
        }
        void OnTriggerStay(Collider other)
        {
            if (other.gameObject == currentTarget)
            {
                RotateTurret();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Enemy")
            {
                _enemy.Dequeue();
                TryNextTarget();         
            }
        
        }

        void TryNextTarget()
        {
            try
            {       
                _nextTarget = _enemy.Peek();              
                currentTarget = _nextTarget;
            }
            catch (Exception)
            {          
                currentTarget = null;
                FireWeapon(false);
            }        
        }

        void RotateTurret()
        {
            Vector3 targetPos = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 1.2f, currentTarget.transform.position.z);
            directionToRotate = targetPos - rotateGunBase.position;
            Debug.DrawRay(rotateGunBase.position, directionToRotate, Color.green);
            targetRotation = Quaternion.LookRotation(directionToRotate, Vector3.up);
            rotateGunBase.rotation = Quaternion.Slerp(rotateGunBase.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        void OnDisable()
        {
            EnemyAI.onEnemyDeath -= EnemyDied;
            EnemyAI.onTurretDamage -= OnTurretDamage;
        }
    }
}

