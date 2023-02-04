using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


[System.Serializable]
[CreateAssetMenu]
public class EnemyWaves : ScriptableObject 
{

    public List<Wave> Waves = new List<Wave>();

    [System.Serializable]
    public class Wave
    {
        public string name;
        public List<EnemyType> enemyGroups;
        public float timeBetweenGroups;

        [Header("Dropbox")]
        public List<GameObject> itemsToDrop;
        public int timeBtnDrops;
    }

    [System.Serializable]
    public class EnemyType 
    {
        public GameObject enemy;
        public bool isGroundTreeEnemy = false;
        public bool isBossEnemy = false;
        public int count;
        public float spawnrate;
    }
}
