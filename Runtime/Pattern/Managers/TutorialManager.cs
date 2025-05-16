using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace RealMethod
{
    public abstract class TutorialManager : MonoBehaviour, IGameManager
    {
        [Serializable]
        public struct Quest
        {
            public string Name;
            public float Time;
        }


        [Header("Dependency")]
        //public CCSaveFile Savefile;
        // public GameObject WallsTitle;
        // public GameObject GameReadyTitle;
        public GameObject RocksTitle;
        public GameObject SpawnTitle;
        [Header("Hint")]
        public Text Hint;
        public float AnimationTime = 2;
        public float AnimationScale = 1.5f;
        [Header("PlayerSetup")]
        // [ShowOnly]
        // public int StartMass = 5;
        // public float JumpPower = 10;
        // [Header("Prefabs")]
        // public GameObject EnemyPrefab;
        [Header("Stage")]
        public Quest[] Quests;



        //private AIManager aIManager;
        private Transform[] Rocks = new Transform[0];
        private Transform[] SpawnPoints = new Transform[0];
        //private float[] PlayerStats = new float[3] { 0, 0, 0 };
        //private bool CanSpawn = true;
        //private bool IsTutorialComplite = false;



        // Implement IGameManager Interface
        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            // if (Savefile == null)
            // {
            //     Savefile = Game.CastWorld<CCWorld>().GetSaveFile();
            // }

            // if (!Savefile.FirstPlay)
            // {
            //     GameReadyTitle.SetActive(false);
            //     BeginTutorial();
            // }
            // else
            // {
            //     GameReadyTitle.SetActive(true);
            //     Destroy(WallsTitle);
            //     Destroy(RocksTitle);
            //     Destroy(SpawnTitle);
            //     Destroy(this);
            // }
        }
        public void InitiateService(Service service)
        {
        }


        // Private Methods
        private void BeginTutorial()
        {
            ShowHint(Quests[0].Name);
            // Game.CastWorld<CCWorld>().OnGameBegin += OnGameStarted;

            // aIManager = Game.World.GetComponent<AIManager>();
            // if (aIManager == null)
            // {
            //     Debug.LogError("AIManager is not found in the world.");
            //     return;
            // }
            // aIManager.SetEnemySpawnEnabled(false);
            // aIManager.OnEnemyDied += OnEnemyDied;


            // foreach (Obstacle item in WallsTitle.GetComponentsInChildren<Obstacle>())
            // {
            //     item.OnDestructedObstacle += OnWallDestry;
            // }


            if (RocksTitle == null)
            {
                Debug.LogError("RocksTitle is not assigned in the Inspector.");
                return;
            }
            // Get only the direct children of RocksTitle
            int childCount = RocksTitle.transform.childCount;
            Rocks = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                RocksTitle.transform.GetChild(i).gameObject.SetActive(false);
                Rocks[i] = RocksTitle.transform.GetChild(i);
            }


            if (SpawnTitle == null)
            {
                Debug.LogError("SpawnTitle is not assigned in the Inspector.");
                return;
            }
            // Get only the direct children of RocksTitle
            int MychildCount = SpawnTitle.transform.childCount;
            SpawnPoints = new Transform[MychildCount];
            for (int i = 0; i < MychildCount; i++)
            {
                SpawnTitle.transform.GetChild(i).gameObject.SetActive(false);
                SpawnPoints[i] = SpawnTitle.transform.GetChild(i);
            }

            SetupPlayer();
        }
        private void SetupPlayer()
        {
            // PlayerCharacter PC = Game.World.GetPlayerObject().GetComponent<PlayerCharacter>();
            // PlayerController PCO = Game.World.GetPlayerObject().GetComponent<PlayerController>();
            // PlayerEnergy PE = Game.World.GetPlayerObject().GetComponent<PlayerEnergy>();
            // PlayerStats[0] = PE.JumpPower;
            // PlayerStats[1] = PCO.swipeThreshold;
            // PE.OnMassModify += OnPlayerMassChanged;
            // PE.AutoStart = false;
            // PE.StartMass = StartMass;
            // PE.JumpPower = JumpPower;
            // PCO.swipeThreshold = 1000000;
            // PC.SetCanTakeDamage(false);
        }
        private void ShowHint(string message)
        {
            Hint.text = message;
            StartCoroutine(ScaleAnimation(Hint.transform, Hint.transform.localScale, Vector3.one * AnimationScale, AnimationTime));
        }
        private void RunQuest(int ID, Action action)
        {
            StartCoroutine(DelayedExecution(Quests[ID].Name, Quests[ID].Time, action));
        }
        private Transform GetSpawnPoint()
        {
            int randomIndex = UnityEngine.Random.Range(0, SpawnPoints.Length);
            Transform spawnPoint = SpawnPoints[randomIndex];
            return spawnPoint;
        }
        private void ResetPlayer()
        {
            // PlayerCharacter PC = Game.World.GetPlayerObject().GetComponent<PlayerCharacter>();
            // PlayerController PCO = Game.World.GetPlayerObject().GetComponent<PlayerController>();
            // PlayerEnergy PE = Game.World.GetPlayerObject().GetComponent<PlayerEnergy>();
            // PE.OnMassModify -= OnPlayerMassChanged;
            // PC.SetCanTakeDamage(true);
            // PE.JumpPower = PlayerStats[0];
            // PCO.swipeThreshold = PlayerStats[1];
        }

        // Events
        private void OnGameStarted()
        {
            //Game.CastWorld<CCWorld>().OnGameBegin -= OnGameStarted;
            RunQuest(1, () =>
            {
                Rocks[0].gameObject.SetActive(true);
                Rocks[1].gameObject.SetActive(true);
                Rocks[2].gameObject.SetActive(true);
                Rocks[3].gameObject.SetActive(true);
                Rocks[4].gameObject.SetActive(true);
            });

        }
        private void OnPlayerMassChanged(float mass)
        {
            // if (mass == StartMass + 3)
            // {
            //     RunQuest(2, () =>
            //     {
            //         aIManager.SpawnEnemy(EnemyPrefab, GetSpawnPoint().position);
            //         RunQuest(3, () =>
            //         {
            //             PlayerController PCO = Game.World.GetPlayerObject().GetComponent<PlayerController>();
            //             PCO.swipeThreshold = PlayerStats[1];


            //             RunQuest(4, () =>
            //             {
            //                 PlayerEnergy PE = Game.World.GetPlayerObject().GetComponent<PlayerEnergy>();
            //                 PE.AddMass(1);
            //             });
            //         });
            //     });
            // }


        }
        // private void OnEnemyDied(Enemy enemy)
        // {
        //     if (CanSpawn)
        //         aIManager.SpawnEnemy(EnemyPrefab, GetSpawnPoint().position);
        // }
        // private void OnWallDestry(Obstacle obst)
        // {
        //     if (IsTutorialComplite)
        //         return;

        //     aIManager.OnEnemyDied -= OnEnemyDied;
        //     IsTutorialComplite = true;
        //     Hint.gameObject.SetActive(false);
        //     aIManager.SetEnemySpawnEnabled(true);
        //     ResetPlayer();
        //     Savefile.FirstPlay = true;
        //     Game.Instance.GetComponent<DataManager>().SaveFile();
        //     Destroy(this);
        // }



        // Coroutines
        private IEnumerator ScaleAnimation(Transform target, Vector3 startScale, Vector3 endScale, float AnimationDuration)
        {
            float halfDuration = AnimationDuration / 2f;
            float elapsedTime;

            // Scale up
            elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                target.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / halfDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            target.localScale = endScale;
            yield return null; // Wait for the end of the frame

            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                target.localScale = Vector3.Lerp(endScale, startScale, elapsedTime / halfDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            target.localScale = startScale;
            yield return null;
        }
        private IEnumerator DelayedExecution(string Message, float delay, Action action)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay
            action?.Invoke(); // Execute the lambda function
            ShowHint(Message);
        }

    }

}
