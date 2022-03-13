using System;
using System.Text;
using MarblePhysics.Modding.Shared;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    [CustomEditor(typeof(MarbleCollisionHandler), true)]
    public class MarbleCollisionHandlerEditor : Editor
    {
        private int lastLayer = -1;
        private MarbleCollisionHandler marbleCollisionHandler;
        private LayerConfig layerConfig;

        private StringBuilder errors = null;

        private bool hasCollisionError = false;
        
        protected virtual void OnEnable()
        {
            errors = new StringBuilder();
            
            marbleCollisionHandler = target as MarbleCollisionHandler;

            layerConfig = StageUtility.GetCurrentStageHandle().FindComponentOfType<LayerConfig>();
        }

        public override void OnInspectorGUI()
        {
            if (layerConfig == null)
            {
                EditorGUILayout.HelpBox("You do not have a LayerConfig set! Please create that first", MessageType.Error);
                return;
            }
            
            base.OnInspectorGUI();
            errors.Clear();
            errors.AppendLine("Errors found!:");
            
            bool hasErrors = false;

            if (!marbleCollisionHandler.TryGetComponent(out Collider2D _))
            {
                errors.AppendLine("- A collision handler requires a Collider2D!");
                hasErrors = true;
            }
            else
            {
                
            }
            
            if (marbleCollisionHandler.gameObject.layer != lastLayer)
            {
                lastLayer = marbleCollisionHandler.gameObject.layer;

                bool anyCollides = false;
                for (int i = 0; i < 32; i++)
                {
                    if (layerConfig.AllMarbleLayers == (layerConfig.AllMarbleLayers | (1 << i)))
                    {
                        anyCollides |= !layerConfig.CollisionMatrix.GetIgnoreLayerCollision(lastLayer, i);
                    }
                }

                hasCollisionError = !anyCollides;
            }

            if (marbleCollisionHandler.EnabledEvents == 0)
            {
                hasErrors = true;
                errors.AppendLine("- This will do nothing if you dont have any events enabled!");
            }

            if (hasCollisionError)
            {
                hasErrors = true;
                errors.AppendLine("- The layer this game object is on can not collide with any of the defined marble layers in your MarbleCollisionHandler!");
            }
        

            if (hasErrors)
            {
                EditorGUILayout.HelpBox(errors.ToString().TrimEnd(), MessageType.Error);
            }
        }
    }
}
