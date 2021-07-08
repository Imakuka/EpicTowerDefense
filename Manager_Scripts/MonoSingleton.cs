using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.TowerDefense
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Debug.Log(typeof(T).ToString() + " Is Null");

                return _instance;
            }
        }

        void Awake()
        {
            _instance = (T)this;
            Debug.Log(this + " Inistialized");
        }

    }

}


