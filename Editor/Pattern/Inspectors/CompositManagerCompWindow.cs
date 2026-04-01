using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    [CustomEditor(typeof(CompositManager), true)]
    public class CompositManagerCompWindow : UnityEditor.Editor
    {
        private CompositManager BaseComponent;

        private void OnEnable()
        {
            BaseComponent = (CompositManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" ----------------- Layers ----------------- ");
            if (BaseComponent != null)
            {
                AudioBehaviour[] layers = BaseComponent.GetLayers();
                if (layers != null && layers.Length > 0)
                {
                    foreach (var layer in layers)
                    {
                        if (layer != null)
                        {
                            EditorGUILayout.LabelField($"{layer.name} - {GetLayerStatus(layer)}");
                            //EditorGUILayout.ObjectField(layer, typeof(AudioBehaviour), true);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No Layers");
                }
            }


        }



        private string GetLayerStatus(AudioBehaviour layer)
        {
            AudioSource source = layer as AudioSource;
            if (source == null)
            {
                return "null";
            }
            if (source.isPlaying)
            {
                return $"playing ({source.time}/{source.clip.length})({source.volume})";
            }

            if (!source.isPlaying)
            {
                return "paused";
            }

            return "Layer is stopped";
        }
    }
}