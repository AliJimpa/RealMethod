
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace RealMethod
{
   [CreateAssetMenu(fileName = "WorldScene", menuName = "Scene/WorldScene", order = 1)]
   public class WorldSceneConfig : ConfigAsset
   {
      [Header("Config")]
      [SerializeField]
      private SceneReference Persistent;
      [SerializeField]
      private SceneReference[] Additive;
      public int AdditiveCount => Additive.Length;

      public SceneReference GetPersistent()
      {
         return Persistent;
      }
      public SceneReference GetAdditive(int index)
      {
         return Additive[index];
      }

      public SceneReference[] GetAllScene()
      {
         SceneReference[] Result = new SceneReference[Additive.Length + 1];
         Result[0] = Persistent;
         for (int i = 0; i < Additive.Length; i++)
         {
            Result[i + 1] = Additive[i];
         }
         return Result;
      }




#if UNITY_EDITOR
      public void OnAssetClick()
      {
         EditorSceneManager.OpenScene(Persistent, OpenSceneMode.Single);
         foreach (var item in Additive)
         {
            EditorSceneManager.OpenScene(item, OpenSceneMode.Additive);
         }
      }
#endif



   }
}