using UnityEngine;

namespace RealMethod
{
    public interface ITutorialSpawner
    {
        ITutorialMessage InstantiateMessage(Transform parent);
    }
    public interface ITutorialMessage
    {
        delegate void Finish();
        event Finish OnFinished;
        T GetClass<T>() where T : UI_Tutorial;
        void Initiate(Object author, W_Tutorial owner, TutorialConfig config);
    }
    public interface ITutorialStorage : IStorage
    {
        void AddNewTutorial(TutorialConfig conf);
        bool RemoveTutorial(TutorialConfig conf);
        bool IsValidTutorial(TutorialConfig conf);
    }
}