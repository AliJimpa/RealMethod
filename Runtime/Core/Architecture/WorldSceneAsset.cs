
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace RealMethod
{
   [CreateAssetMenu(fileName = "WorldScene", menuName = "Scene/WorldScene", order = 1)]
   public class WorldSceneAsset : DataAsset
   {
      [Header("Config")]
      [SerializeField]
      private SceneReference Persistent;
      [SerializeField]
      private List<SceneReference> Additive;

      public SceneReference GetPersistent()
      {
         return Persistent;
      }
      public SceneReference GetAdditive(int index)
      {
         return Additive[index];
      }
      public int GetAdditiveCount()
      {
         return Additive.Count;
      }

      public SceneReference[] GetAllScene()
      {
         SceneReference[] Result = new SceneReference[Additive.Count + 1];
         Result[0] = Persistent;
         for (int i = 0; i < Additive.Count; i++)
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