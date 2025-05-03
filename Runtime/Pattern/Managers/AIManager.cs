using UnityEngine;

namespace RealMethod
{
    public abstract class AIManager : MonoBehaviour, IGameManager
    {

        // private int availableTokens;

        // [Header("Enemy Generation")]
        // [SerializeField]
        // private bool spawnPermission = true;
        // [SerializeField]
        // List<EnemyData> difficultyData;
        // [SerializeField, Tooltip("the Mask to Detect if Anything is In Between the Enemy (in SpawnPosition) and Player, or it is spawning in Rocks, Obstacles etc.")]
        // private LayerMask obstacleMask;
        // [SerializeField]
        // private ParticlePool spawnEffect;

        // public Action<Enemy> OnEnemyDied;
        // private int maxEnemiesCount;

        // private List<EnemyDifficultyData> enemiesToSpawn;

        // private List<Enemy> Enemies = new List<Enemy>(); // List of all of the existing enemies


        // private CCWorld MyWorld;
        // private PlayerCharacter player;
        // private bool IsGameStarted = false;

        // float lastSpawnTime;

        // float spawnCooldown;

        // public float minDistance = 5f;
        // public float maxDistance = 20f;
        // public int maxAttempts = 6; // Max attempts to find a valid spawn point
        // public int coneAngle = 45; // Angle for the cone in degrees
        // public float speedThreshold = 5f; // Threshold for checking if player is near zero velocity

        // #region Manager Section

        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            if (AlwaysLoaded) return;

            // MyWorld = GetComponent<CCWorld>();
            // if (MyWorld == null) Debug.LogError("AIDirector Couldn't Find World.");
            // player = MyWorld.GetPlayerCharacter();
            // MyWorld.OndifficultyChange += UpdateDifficultyData;
            // MyWorld.OnGameBegin += OnGameStarted;

        }
        public void InitiateService(Service service)
        {
            throw new System.NotImplementedException();
        }

        // #endregion

        // void Update()
        // {
        //     if (IsGameStarted)
        //     {
        //         HandleEnemySpawning();
        //     }

        // }



        // private void HandleEnemySpawning()
        // {
        //     if (!spawnPermission) return;

        //     if (Enemies.Count < maxEnemiesCount)
        //     {
        //         if (Time.time > lastSpawnTime + spawnCooldown)
        //         {
        //             GameObject randomEnemy = GetRandomEnemy();
        //             if (randomEnemy == null) return; // No enemy found, exit early

        //             Vector3 spawnPos = Vector3.zero;

        //             bool foundPos = FindSpawnPositionInCone(out spawnPos, randomEnemy);
        //             if (!foundPos) return;



        //             if (randomEnemy != null)
        //                 SpawnEnemy(randomEnemy, spawnPos);
        //             else Debug.LogError("No Enemy Found in Current Level Difficulty!");
        //         }
        //     }
        // }
        // private GameObject GetRandomEnemy()
        // {
        //     float totalWeight = 0f;
        //     foreach (var weight in enemiesToSpawn)
        //     {
        //         totalWeight += weight.spawnWeight;
        //     }

        //     // Step 2: Generate a random value between 0 and totalWeight
        //     float randomValue = UnityEngine.Random.Range(0, totalWeight);

        //     // Step 3: Select an enemy based on the random value
        //     float accumulatedWeight = 0f;
        //     foreach (var enemyData in enemiesToSpawn)
        //     {
        //         accumulatedWeight += enemyData.spawnWeight;

        //         // When the accumulated weight exceeds or matches the random value, we have our enemy
        //         if (randomValue <= accumulatedWeight)
        //         {
        //             return enemyData.enemy; // Return the selected enemy GameObject
        //         }
        //     }

        //     // In case something goes wrong, return null (should not happen if weights are set correctly)
        //     return null;
        // }
        // public Enemy SpawnEnemy(GameObject enemyToSpawn, Vector3 spawnLoc)
        // {
        //     spawnEffect.Spawn(spawnLoc, spawnEffect.Particle.transform.rotation, spawnEffect.Particle.transform.localScale);
        //     GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnLoc, Quaternion.identity);

        //     Enemy target = spawnedEnemy.GetComponent<Enemy>();
        //     Enemies.Add(target);

        //     OffScreenIndicator.TargetStateChanged(spawnedEnemy.GetComponent<Target>(), true);

        //     lastSpawnTime = Time.time;
        //     return target;
        // }

        // public void DestroyEnemy(Enemy enemy)
        // {
        //     Enemies.Remove(enemy);
        //     OffScreenIndicator.TargetStateChanged(enemy.gameObject.GetComponent<Target>(), false);
        //     OnEnemyDied?.Invoke(enemy);
        //     Destroy(enemy.gameObject);
        // }

        // #region Enemy Spawning

        // public void SetEnemySpawnEnabled(bool enabled)
        // {
        //     spawnPermission = enabled;
        // }
        // bool FindSpawnPositionInCone(out Vector3 outPosition)
        // {
        //     Vector3 cameraForward = player.Camera_Obj.transform.forward;
        //     cameraForward.y = 0;
        //     cameraForward.Normalize();

        //     for (int i = 0; i < maxAttempts; i++) // Count attempts
        //     {
        //         Vector3 spawnDirection = GetRandomConeDirection(cameraForward, coneAngle);
        //         float spawnDistance = UnityEngine.Random.Range(minDistance, maxDistance);
        //         Vector3 spawnPosition = player.transform.position + spawnDirection * spawnDistance;
        //         spawnPosition.y = 1;

        //         // Ensure the spawn position is valid using SphereCast
        //         if (IsValidSpawnPosition(spawnPosition))
        //         {
        //             outPosition = spawnPosition;
        //             return true;
        //         }
        //     }
        //     outPosition = Vector3.zero;
        //     return false;
        // }

        // bool FindSpawnPositionInCone(out Vector3 outPosition, GameObject SpawningEnemy)
        // {
        //     Vector3 cameraForward = player.Camera_Obj.transform.forward;
        //     cameraForward.y = 0;
        //     cameraForward.Normalize();

        //     for (int i = 0; i < maxAttempts; i++) // Count attempts
        //     {
        //         Vector3 spawnDirection = GetRandomConeDirection(cameraForward, coneAngle);
        //         float spawnDistance = UnityEngine.Random.Range(minDistance, maxDistance);
        //         Vector3 spawnPosition = player.transform.position + spawnDirection * spawnDistance;
        //         spawnPosition.y = 1;

        //         // Ensure the spawn position is valid using SphereCast
        //         if (IsValidSpawnPosition(spawnPosition, SpawningEnemy))
        //         {
        //             outPosition = spawnPosition;
        //             return true;
        //         }
        //     }
        //     outPosition = Vector3.zero;
        //     return false;
        // }

        // bool FindSpawnPositionInCircle(out Vector3 outPosition)
        // {
        //     for (int i = 0; i < maxAttempts; i++)
        //     {
        //         Vector3 spawnDirection = UnityEngine.Random.insideUnitSphere;
        //         spawnDirection.y = 0;
        //         spawnDirection.Normalize();
        //         float spawnDistance = UnityEngine.Random.Range(minDistance, maxDistance);
        //         Vector3 spawnPosition = player.transform.position + spawnDirection * spawnDistance;
        //         spawnPosition.y = 1;

        //         // Ensure the spawn position is valid using SphereCast
        //         if (IsValidSpawnPosition(spawnPosition))
        //         {
        //             outPosition = spawnPosition;
        //             return true;
        //         }
        //     }
        //     outPosition = Vector3.zero;
        //     return false;
        // }

        // Vector3 GetRandomConeDirection(Vector3 forwardDirection, float coneAngle)
        // {
        //     // Get a random angle within the cone range
        //     float randomAngle = UnityEngine.Random.Range(-coneAngle / 2, coneAngle / 2);
        //     Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        //     return rotation * forwardDirection;
        // }
        // private bool IsValidSpawnPosition(Vector3 position)
        // {
        //     // Check line of sight to player
        //     Vector3 directionToPlayer = player.transform.position - position;
        //     float distanceToPlayer = directionToPlayer.magnitude;

        //     if (Physics.Raycast(position, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
        //     {
        //         // Something is blocking the view to the player
        //         return false;
        //     }

        //     return true;
        // }
        // private bool IsValidSpawnPosition(Vector3 position, GameObject SpawningEnemy)
        // {
        //     // Check line of sight to player
        //     Vector3 directionToPlayer = player.transform.position - position;
        //     float distanceToPlayer = directionToPlayer.magnitude;

        //     if (Physics.Raycast(position, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
        //     {
        //         // Something is blocking the view to the player
        //         return false;
        //     }

        //     // Check for Destructibles in spawn radius
        //     Collider[] hits = Physics.OverlapSphere(position, SpawningEnemy.transform.localScale.x, obstacleMask);
        //     if (hits.Length > 0)
        //     {
        //         // Found a destructible object in the area
        //         return false;
        //     }

        //     return true;
        // }

        // #endregion

        // #region Token-Handling
        // public bool RequestToken(EnemyController enemyController, int count)
        // {
        //     if (availableTokens >= count)
        //     {
        //         availableTokens -= count;
        //         enemyController.AddToken(count);
        //         return true;
        //     }
        //     return false;
        // }
        // public void OnTokenReleased(int releasedTokens)
        // {
        //     availableTokens += releasedTokens;
        // }

        // #endregion

        // public void UpdateDifficultyData(int newLevel)
        // {
        //     // Return if list is null
        //     if (difficultyData.Count == 0)
        //     {
        //         Debug.LogError("No difficulty data has been set!");
        //         return;
        //     }

        //     int index = Mathf.Clamp(newLevel, 1, difficultyData.Count - 1);

        //     EnemyData currentDifficultyData = difficultyData[index];
        //     enemiesToSpawn = currentDifficultyData.enemiesToSpawn;

        //     maxEnemiesCount = currentDifficultyData.MaxEnemiesToSpawn;
        //     availableTokens = currentDifficultyData.AvailableTokens;
        //     spawnCooldown = currentDifficultyData.SpawnCooldown;
        // }

        // private void OnGameStarted()
        // {
        //     IsGameStarted = true;
        // }
    }

}
