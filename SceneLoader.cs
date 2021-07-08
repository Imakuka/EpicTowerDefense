using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameDevHQ.TowerDefense
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadGameScene()
        {
            SceneManager.LoadScene(1);
        }
    }
}

