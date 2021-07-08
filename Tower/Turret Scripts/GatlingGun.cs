using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameDevHQ.TowerDefense
{
    
    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class GatlingGun : TurretControl
    {
        
        [Header("Gun Components")]
        [SerializeField]
        private Transform[] _gunBarrel; //Reference to hold the gun barrel
        [SerializeField]
        private GameObject[] _muzzleFlash; //reference to the muzzle flash effect to play when firing
        [SerializeField]
        private ParticleSystem[] _bulletCasings; //reference to the bullet casing effect to play when firing
        [SerializeField]
        private float _barrelRotationSpeed = -500.0f;
        [Header("Gun Audio")]
        [SerializeField]
        private AudioClip _fireSound; //Reference to the audio clip
        [SerializeField]
        private AudioSource _audioSource; //reference to the audio source component

        private bool _startWeaponNoise = true;
        private bool _weCanFire;
        private WaitForSeconds _damageYield;
        private float _damageDelay = 1.0f;

        // Use this for initialization
        void Start()
        {
            DefineComponents();         
        }

        void DefineComponents()
        {
            for (int i = 0; i < _muzzleFlash.Length; i++)
            {
                _muzzleFlash[i].SetActive(false);
            }

            _damageYield = new WaitForSeconds(_damageDelay);
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
            _audioSource.clip = _fireSound;
        }

        // Update is called once per frame
        void Update()
        {
            RotateBarrel();
        }

        IEnumerator DamageRoutine()
        {
            while (currentTarget != null)
            {
                OnTargetToDamage();
                yield return _damageYield;
            }
        }


        protected override void FireWeapon(bool canWeFire)
        {
            if (canWeFire == true)
            {
                _weCanFire = canWeFire;
                for (int i = 0; i < _muzzleFlash.Length; i++)
                {
                    _muzzleFlash[i].SetActive(true);
                    _bulletCasings[i].Emit(1);
                }

                if (_startWeaponNoise == true)
                {
                    _audioSource.Play();
                    _startWeaponNoise = false;
                }
                StartCoroutine(DamageRoutine());
            }
            else
            {
                for (int i = 0; i < _muzzleFlash.Length; i++)
                {
                    _muzzleFlash[i].SetActive(false);
                }
                _audioSource.Stop();
                _startWeaponNoise = true;
            }
        }

        void RotateBarrel()
        {

            for (int i = 0; i < _gunBarrel.Length; i++)
            {
                if (_weCanFire == true)
                {
                    _gunBarrel[i].transform.Rotate(Vector3.forward * Time.deltaTime * _barrelRotationSpeed);
                }
                else
                {
                    _gunBarrel[i].transform.Rotate(Vector3.zero * Time.deltaTime * 0);
                }
            }

        }

    }
}



