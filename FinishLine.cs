using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;


namespace GameDevHQ.TowerDefense
{
    public class FinishLine : MonoBehaviour      
    {
        public static event Action<bool> onFinishEnter;

        [SerializeField]
        private Transform _start;
 
        private bool _atFinish = false;
        // Start is called before the first frame update
        void Awake()
        {
            
        }

        void Start()
        {
            DefineComponents();
        }

        void DefineComponents()
        {
            _start = GameManager.Instance.GetStart();
            if (_start == null)
            {
                Debug.Log("The Start Pos is Null");
            }
        }

        void AtFinishLine(bool isAtFinish)
        {
            _atFinish = isAtFinish;

            if (onFinishEnter != null)
            {
                onFinishEnter(_atFinish);
            }
        }

        // Update is called once per frame
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                AtFinishLine(true);     
                other.gameObject.SetActive(false);
                GameManager.Instance.PlayerHealth(10);
                NavMeshAgent enemy = other.gameObject.GetComponent<NavMeshAgent>();
                if (enemy != null)
                {
                    enemy.Warp(_start.position);
                }
                other.gameObject.SetActive(true);
                
            }
        }
    }
}

