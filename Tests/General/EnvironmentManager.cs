using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace RealMethod
{
    public abstract class EnvironmentManager : MonoBehaviour, IGameManager
    {
        private int PlayerLevel;

        [Header("Storm")]
        [SerializeField] private PrefabPool PillarPrefab;
        [SerializeField] private int StormLevelThreshold = 3;
        [SerializeField] private float StormInterval = 100f;
        [SerializeField] private float StormTimeDeviation = 15f;
        [SerializeField] private float BaseStormDuration = 10f;
        [SerializeField] private float StormDurationExtendPerLevel = 2.5f;
        [SerializeField] private int BasePillarCount = 10;
        [SerializeField] private int ExtraPillarPerLevel = 3;
        // [SerializeField] private float PredictionTime = 1f;
        // [SerializeField] private float RandomRadius = 0f;

        //[SerializeField] private Volume PostProcessingVolume;

        private float StormTimeElapsed;

        #region Manager Section

        // Returns the current instance of the manager class
        public MonoBehaviour GetManagerClass()
        {
            return this;
        }

        // Initializes the manager and subscribes to the difficulty change event
        public void InitiateManager(bool AlwaysLoaded)
        {
            //CCGame.CastWorld<CCWorld>().OndifficultyChange += OnPlayerLevelChanged;
        }

        // Placeholder for initiating services, currently does nothing
        public void ResolveService(Service service, bool active)
        {
        }

        #endregion

        // Event handler for player level change
        private void OnPlayerLevelChanged(int newLevel)
        {
            PlayerLevel = newLevel;

            if (PlayerLevel >= StormLevelThreshold)
            {
                QueueStorm(UnityEngine.Random.Range(0, StormTimeDeviation));
                //CCGame.CastWorld<CCWorld>().OndifficultyChange -= OnPlayerLevelChanged;
            }
        }

        #region Storm


        // Schedules the next storm based on the interval and deviation
        private void QueueStorm()
        {
            float stormDeviation = UnityEngine.Random.Range(-StormTimeDeviation, StormTimeDeviation);
            Invoke(nameof(StartStorm), StormInterval + stormDeviation);
        }
        private void QueueStorm(float timer)
        {
            Invoke(nameof(StartStorm), timer);
        }

        // Starts the storm coroutine
        public void StartStorm()
        {
            StartCoroutine(ProceedStorm());
        }

        // Coroutine to handle the storm process
        private IEnumerator ProceedStorm()
        {
            StormTimeElapsed = 0;
            int calculatedPillarCount = BasePillarCount + ExtraPillarPerLevel * PlayerLevel;
            float calculatedStormDuration = BaseStormDuration + StormDurationExtendPerLevel * PlayerLevel;
            float delayBetweenPillars = calculatedStormDuration / calculatedPillarCount;

            // Blend PostExposure to the storm value
            //float stormPostExposureValue = -1.5f; // Example value for storm intensity
            //float initialPostExposureValue = 0.2f; // Default value
            // if (PostProcessingVolume != null)
            // {
            //     BlendPostExposure(initialPostExposureValue, stormPostExposureValue, 1.5f);
            //     yield return new WaitForSeconds(2f);
            // }

            while (StormTimeElapsed <= calculatedStormDuration)
            {
                StormTimeElapsed += delayBetweenPillars;
                PillarPrefab.Spawn(FindPredictedLocation());
                yield return new WaitForSeconds(delayBetweenPillars);
            }

            // Blend PostExposure back to the initial value
            // if (PostProcessingVolume != null)
            // {
            //     BlendPostExposure(stormPostExposureValue, initialPostExposureValue, 1.5f);
            // }

            QueueStorm();
        }

        // private void BlendPostExposure(float fromValue, float toValue, float duration)
        // {
        //     if (PostProcessingVolume.profile.TryGet(out UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments))
        //     {
        //         StartCoroutine(BlendPostExposureCoroutine(colorAdjustments, fromValue, toValue, duration));
        //     }
        // }

        // private IEnumerator BlendPostExposureCoroutine(UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments, float fromValue, float toValue, float duration)
        // {
        //     float elapsedTime = 0f;

        //     while (elapsedTime < duration)
        //     {
        //         elapsedTime += Time.deltaTime;
        //         float t = Mathf.Clamp01(elapsedTime / duration);
        //         colorAdjustments.postExposure.value = Mathf.Lerp(fromValue, toValue, t);
        //         yield return null;
        //     }

        //     colorAdjustments.postExposure.value = toValue;
        // }

        // Predicts the location where the next pillar should spawn
        private Vector3 FindPredictedLocation()
        {
            //GameObject player = CCGame.World.GetPlayerObject();
            //Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
            //Vector3 predictedLocation = player.transform.position + playerVelocity * PredictionTime;
            //predictedLocation += Random.insideUnitSphere * RandomRadius;
            //predictedLocation.y = 0;

            //return predictedLocation;
            return Vector3.one;
        }

        #endregion
    }

}
