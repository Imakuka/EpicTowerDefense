
using UnityEngine;

namespace GameDevHQ.TowerDefense
{
    [CreateAssetMenu(menuName = "Scriptable Obj/New Wave")]
    public class Wave : ScriptableObject
    {
        public bool randomOn;
        public int amountToSpawn;
        public int enemyTypes; // 2 enemies so far
        public int[] fixedSpawn;

        public int ReturnEnemyType(int elementNo)
        {
            enemyTypes = PoolManager.Instance.ReturnEnemyTypeLength();
            int nextEnemyType;
            if (randomOn == true)
            {
                nextEnemyType = Random.Range(0, enemyTypes);
            }
            else
            {
                nextEnemyType = fixedSpawn[elementNo];
            }

            return nextEnemyType;
        }

    }
}


