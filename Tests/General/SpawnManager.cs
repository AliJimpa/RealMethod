using UnityEngine;

namespace RealMethod
{
    public abstract class EnemySpawner : MonoBehaviour, IGameManager
    {
        public GameObject[] enemyPrefabs;
        public float Rediosspawn = 1;
        public int FirstWave = 1;

        // Start is called before the first frame update
        void Start()
        {
            SpawnEnemyWave(FirstWave);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SpawnEnemyWave(int enemiesToSpawn)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                int randomEnemy = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                Instantiate(enemyPrefabs[randomEnemy], GenerateSpawnPosition(),
                enemyPrefabs[randomEnemy].transform.rotation);
            }
        }

        Vector3 GenerateSpawnPosition()
        {
            float x = UnityEngine.Random.Range(-Rediosspawn, Rediosspawn);
            float z = UnityEngine.Random.Range(-Rediosspawn, Rediosspawn);
            return new Vector3(x, transform.position.y, z);
        }

        public void StartManager(World WorldTarget)
        {
            throw new System.NotImplementedException();
        }
        public void StopManager()
        {
            throw new System.NotImplementedException();
        }

        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            throw new System.NotImplementedException();
        }
        public void InitiateService(Service service)
        {
            throw new System.NotImplementedException();
        }


    }

}
