using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RealMethod
{
    public abstract class MissionManager : MonoBehaviour, IGameManager
    {
        [Header("Mission")]
        public int MissionID = 0;
        public bool LimitSpeed = false;
        [ConditionalHide("LimitSpeed", true, false)]
        public float NewSpeed = 30;
        public bool SetNewCameraSetting = false;
        [ConditionalHide("SetNewCameraSetting", true, false)]
        public float NewDistance = 40;
        [Header("Goal")]
        public int ObstacleCount = 1;
        public string GoalTitleText = "Goal Title";
        [Header("Dependency")]
        //public CCSaveFile SaveFile;
        //public CCGameData GameData;
        public GameObject UI;
        public Animator playerStatusAnimator;
        public GameObject MissionPassUI;
        public SceneReference HomeScene;
        [Header("Events")]
        public UnityEvent<MissionManager> OnMissionCompleted;


        //private CCWorld MyWorld;
        //private PlayerController playercontroller;
        //private PlayerEnergy playerenargy;
        // private Rigidbody body;
        private int currentObstacleCount = 0;
        // private bool IsMissionComplited = false;
        private DataManager dataManager;
        private Text GoalRecord;
        private Text GoalTitle;
        private Button NextLevel;




        // IGameManager Interface Implementation
        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            //MyWorld = Game.CastWorld<CCWorld>();
            //GameObject PlayerObject = MyWorld.GetPlayerObject();
            //playercontroller = PlayerObject.GetComponent<PlayerController>();
            //playerenargy = PlayerObject.GetComponent<PlayerEnergy>();
            //body = PlayerObject.GetComponent<Rigidbody>();
            dataManager = Game.Instance.GetComponent<DataManager>();
            GoalRecord = UI.GetComponentsInChildren<Text>()[1];
            GoalTitle = UI.GetComponentsInChildren<Text>()[0];
            GoalRecord.text = $"{currentObstacleCount}/{ObstacleCount}";
            GoalTitle.text = GoalTitleText;
            //MyWorld.OndifficultyChange += SetupPlayer;
            NextLevel = MissionPassUI.transform.Find("B_NextMission").GetComponent<Button>();
            //if (GameData.GetMissionsLength() == MissionID)
            //NextLevel.gameObject.SetActive(false);
        }
        public void ResolveService(Service service, bool active)
        {
        }



        private void OnEnable()
        {
            //playerenargy.OnDestructedObstacle += OnObstacleDestruct;
        }
        private void OnDisable()
        {
            //playerenargy.OnDestructedObstacle -= OnObstacleDestruct;
        }
        public void OpenNextLevel()
        {
            //GameData.OpenMission(MissionID + 1);
        }
        public void OpenHomeScene()
        {
            //////////////////////Game.OpenScene(HomeScene);
        }


        // private void OnObstacleDestruct(Obstacle other)
        // {
        //     if (IsMissionComplited) return;

        //     currentObstacleCount++;
        //     GoalRecord.text = $"{currentObstacleCount}/{ObstacleCount}";
        //     if (currentObstacleCount >= ObstacleCount)
        //     {
        //         IsMissionComplited = true;

        //         if (SaveFile.Mission == MissionID)
        //             SaveFile.Mission++;

        //         dataManager.SaveFile(SaveFile);

        //         StartCoroutine(MissionComplited(2));
        //     }
        // }
        // private void SetupPlayer(int currentLevel)
        // {
        //     MyWorld.OndifficultyChange -= SetupPlayer;

        //     // MyWorld.SetDifficultyLevelNumber(StartDifficultyLevel);
        //     // playerStatus.text = StartDifficultyLevel.ToString();

        //     //float MinScale = GameData.PlayerScaleByLevel.Evaluate(StartDifficultyLevel);
        //     // float MaxScale = GameData.PlayerScaleByLevel.Evaluate(StartDifficultyLevel + 1);
        //     //playerenargy.SetStartMass(StartMass);
        //     if (LimitSpeed)
        //         playerenargy.SettingUp(NewSpeed);

        //     if (SetNewCameraSetting)
        //         MyWorld.GetPlayerObject().GetComponent<PlayerCharacter>().GetCameraDistance().Distance = NewDistance;

        //     // Update Scale & Camera
        //     //float NewScale = GameData.PlayerScaleByLevel.Evaluate(StartMass);
        //     //playercontroller.transform.localScale = Vector3.one * NewScale;
        // }


        // IEnumerator MissionComplited(float delay)
        // {
        //     playerStatusAnimator.Play("MissionDone");
        //     playercontroller.enabled = false;
        //     yield return new WaitForSeconds(delay);
        //     playerenargy.enabled = false;
        //     body.isKinematic = true;
        //     MissionPassUI.SetActive(true);

        //     // Call Mission Completed Event
        //     OnMissionCompleted?.Invoke(this);
        // }

    }

}
