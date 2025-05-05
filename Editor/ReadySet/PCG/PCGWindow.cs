using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RealMethod
{

    public class PCGWindow : EditorWindow
    {
        // Add the menu item
        [MenuItem("Window/RealMethod/PCG")]
        public static void ShowWindow()
        {
           GetWindow<PCGWindow>("PCG Window");
        }

        private void OnGUI()
        {
            
        }

        


    }

}