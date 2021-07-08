using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;


namespace GameDevHQ.TowerDefense
{
    public class EnemyAI : Mech
    {

        public static event Action<GameObject> onEnemyDeath;
        public static event Action<GameObject, int> onTurretDamage;

        [Header("Nav Mesh")]
        [SerializeField]
        private Transform _finish;       
        [SerializeField]
        private Transform _start;
        [SerializeField]
        private NavMeshAgent _mechNav;
        [SerializeField]
        private float _mechSpeed = 1.5f;

        [Header("Animation")]
        [SerializeField]
        private Animator _mechAnimator;

        [Header("Rotate Functions")]
        [SerializeField]
        private Transform _rotateBody;
        [SerializeField]
        private GameObject _target;
        private Vector3 _directionToRotate;
        private Quaternion _targetRotation;

        [Header("Dissolve Functions")]
        [SerializeField]
        private Renderer[] _mechRends;
        [SerializeField]
        private float _dissolveValue;
        [SerializeField]
        private Collider _mechCollider;
        [SerializeField]
        private Rigidbody _mechRigidbody;

        [Header("Attack")]
        [SerializeField]
        private int _damageArmor;

        private WaitForSeconds _deathAnimYield;
        private float _deathAnimOffset = 2.5f;
        private WaitForSeconds _dissolveAnimYield;
        private float _dissolveAnimOffset = 1.5f;
        private WaitForEndOfFrame _dissolveYield;

        void Awake()
        {           
            DefineComponents();

        }
        void OnEnable()
        {
            OnReEnable();
            TurretControl.onTargetToDamage += TakingDamage;
            Missle.onTargetCollision += TakingDamage;
        }

  
        void DefineComponents()
        {
            _mechNav = GetComponent<NavMeshAgent>();
            if (_mechNav != null)
            {
                _mechNav.speed = _mechSpeed;
            }

            _finish = GameManager.Instance.GetFinish();
            _start = GameManager.Instance.GetStart();         
            _mechRends = GetComponentsInChildren<Renderer>();
            _deathAnimYield = new WaitForSeconds(_deathAnimOffset);
            _dissolveAnimYield = new WaitForSeconds(_dissolveAnimOffset);
            _dissolveYield = new WaitForEndOfFrame();
        }


        public void TakingDamage(GameObject activeTarget, int damageAmount)//called from Turret Control
        {
            _healthRend.material.SetFloat("_health", _currentHealth);
            _damageToTake = damageAmount;
            if (gameObject == activeTarget && _currentHealth > 1)
            {
                
                int health = _currentHealth - damageAmount;
                _currentHealth = health;         
            }

            if (_currentHealth <= 0)//check if I am dead
            {
                _currentHealth = 0;
                StartCoroutine(OnEnemyDeathRoutine());
            }
        }

        void DamageTurretRoutine()
        {
            if (_target != null)
            {
                if (onTurretDamage != null)
                {
                    onTurretDamage(_target, _damageArmor);
                }
            }
        }

        void RotateBody()
        {
            _mechAnimator.SetBool("CanShoot", true);
            
            _rotateBody.transform.LookAt(_target.transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "TowerGun")
            {
                _target = other.gameObject;
                DamageTurretRoutine();
                
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "TowerGun")
            {
                _target = other.gameObject;
                RotateBody();
            }

        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "TowerGun")
            {
                
                _rotateBody.rotation = transform.rotation;
                _target = null;
                if (_target == null)
                {
                    _mechAnimator.SetBool("CanShoot", false);
                }
            }
        }

        IEnumerator OnEnemyDeathRoutine()
        {
            MechDied(gameObject);
            
            _mechAnimator.SetTrigger("OnEnemyDeath");
            _mechCollider.enabled = false;
            _mechNav.SetDestination(transform.position);

            yield return _deathAnimYield;

            StartCoroutine(DissolveRoutine());

            GameManager.Instance.OnMechDeath(_warProfits);

            yield return _dissolveAnimYield;// delay is for animation
            OnDeathReset();
            //yield return new WaitForSeconds(1.0f);
        }

        IEnumerator DissolveRoutine()
        {
            _dissolveValue = 0;

            while(_dissolveValue < 1)
            {
                _dissolveValue += 0.01f;

                for(int i = 0; i <_mechRends.Length; i++)
                {
                    _mechRends[i].material.SetFloat("_fillAmount" , _dissolveValue);
                }
                yield return _dissolveYield;
            }

        }

        public void MechDied(GameObject mechWhoDied) // event saying who died for anyone listening 
        {
            if (onEnemyDeath != null)
            {
                onEnemyDeath(mechWhoDied);
            }
        }
        void OnDeathReset()
        {
            _mechAnimator.WriteDefaultValues();
            gameObject.SetActive(false);
            _mechNav.Warp(_start.position);
            _mechCollider.enabled = true;
            SpawnManager.Instance.WaveComplete();

            for (int i = 0; i < _mechRends.Length; i++)
            {
                _mechRends[i].material.SetFloat("_fillAmount", 0);
            }
        }

        void OnReEnable()
        {
            
            if (_currentHealth > 1)
            {
                _mechNav.SetDestination(_finish.position);
            }
            else // mech has died
            {
                _currentHealth = _health;
                _mechNav.SetDestination(_finish.position);
            }
            
        }

        void OnDisable()
        {
            TurretControl.onTargetToDamage -= TakingDamage;
            Missle.onTargetCollision -= TakingDamage;
        }




    }
}


