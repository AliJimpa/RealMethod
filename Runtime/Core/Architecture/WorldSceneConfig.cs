
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace RealMethod
{
   [CreateAssetMenu(fileName = "WorldScene", menuName = "Scene/WorldScene", order = 1)]
   public class WorldSceneConfig : ConfigAsset
   {
      [Header("Scenes")]
      [SerializeField]
      private SceneReference persistent;
      public SceneReference Persistent => persistent;
      [SerializeField]
      private SceneReference[] Layers;
      public int Count => Layers.Length;

      public SceneReference this[int index]
      {
         get => Layers[index];
      }

#if UNITY_EDITOR
      public void OnAssetClick()
      {
         EditorSceneManager.OpenScene(Persistent, OpenSceneMode.Single);
         foreach (var item in Layers)
         {
            EditorSceneManager.OpenScene(item, OpenSceneMode.Additive);
         }
      }
#endif



   }
}