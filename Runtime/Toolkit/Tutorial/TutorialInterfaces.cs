using UnityEngine;

namespace RealMethod
{
    public interface ITutorialSpawner
    {
        ITutorialMessage InstantiateObject(Transform parent);
    }
    public interface ITutorialMessage
    {
        delegate void Finish();
        event Finish OnFinished;
        MonoBehaviour GetClass();
        void Initiate(Object author, Tutorial owner, TutorialConfig config);
        void SetPosition(Vector3 position, bool isWorld, TutorialPlacement placment, float bufferOffset);
    }

    public interface ITutorialStorage : IStorage
    {
        void AddNewTutorial(TutorialConfig conf);
        bool RemoveTutorial(TutorialConfig conf);
        bool IsValidTutorial(TutorialConfig conf);
    }
}