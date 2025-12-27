
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace RealMethod
{
   /// <summary>
   /// ScriptableObject for configuring a world scene setup in Unity.
   /// Allows specifying a persistent scene and multiple additive layers.
   /// </summary>
   [CreateAssetMenu(fileName = "WorldScene", menuName = "Scene/WorldScene", order = 1)]
   public class WorldSceneConfig : ConfigAsset
   {
      [Header("Scenes")]
      /// <summary>
      /// The persistent scene that is always loaded and acts as the base scene.
      /// </summary>
      [SerializeField]
      private SceneReference persistent;
      /// <summary>
      /// Public getter for the persistent scene.
      /// </summary>
      public SceneReference Persistent => persistent;
      /// <summary>
      /// Array of additive layer scenes that can be loaded alongside the persistent scene.
      /// </summary>
      [SerializeField]
      private SceneReference[] Layers;
      /// <summary>
      /// Gets the number of additive layer scenes.
      /// </summary>
      public int Count => Layers.Length;
      /// <summary>
      /// Indexer to access additive layer scenes by index.
      /// </summary>
      /// <param name="index">Index of the layer scene.</param>
      /// <returns>The <see cref="SceneReference"/> at the specified index.</returns>
      public SceneReference this[int index] => Layers[index];




#if UNITY_EDITOR
      /// <summary>
      /// Opens the persistent scene and all additive layer scenes in the editor.
      /// Only available in the Unity Editor.
      /// </summary>
      public void OnAssetClick()
      {
         // Open the persistent base scene in Single mode
         EditorSceneManager.OpenScene(Persistent, OpenSceneMode.Single);

         // Open all layer scenes additively
         foreach (var item in Layers)
         {
            EditorSceneManager.OpenScene(item, OpenSceneMode.Additive);
         }
      }
#endif
   }
}