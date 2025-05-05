using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace RealMethod
{

    public class PCGWindow : EditorWindow
    {
        private EP_ScriptableObject<PCGResource> Resurce;
        private EP_ScriptableObject<PCGGeneration> Genration;
        private EP_ScriptableObject<PCGCash> Cash;


        // Add the menu item
        [MenuItem("Window/RealMethod/PCG")]
        public static void ShowWindow()
        {
            GetWindow<PCGWindow>("PCG Window");
        }


        private void OnEnable()
        {
            Resurce = new EP_ScriptableObject<PCGResource>("Resurce", this);
            Genration = new EP_ScriptableObject<PCGGeneration>("Genration", this);
            Cash = new EP_ScriptableObject<PCGCash>("Cash", this);
        }

        private void OnGUI()
        {
            Resurce.Render();
            Genration.Render();
            Cash.Render();
        }




    }

}