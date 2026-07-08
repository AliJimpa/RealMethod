using UnityEngine;

namespace RealMethod
{
    public interface IWidget
    {
        MonoBehaviour GetWidgetClass();
        void SceneInitialized(UIManager manager);
    }
}