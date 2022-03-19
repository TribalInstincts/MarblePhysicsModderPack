using System.Diagnostics.Eventing.Reader;
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
        private MarbleCollisionHandler collisionHandler;
        private LayerConfig layerConfig;

        private StringBuilder errors = null;

        private bool hasCollisionError = false;

        public MarbleCollisionHandler.Events RestrictedEvents;
        public bool SingleEvent = false;
        public bool RequireTrigger = false;

        protected virtual void OnEnable()
        {
            RestrictedEvents = MarbleCollisionHandler.Events.Enter | MarbleCollisionHandler.Events.Stay | MarbleCollisionHandler.Events.Exit;
            errors = new StringBuilder();
            
            collisionHandler = target as MarbleCollisionHandler;

            layerConfig = StageUtility.GetCurrentStageHandle().FindComponentOfType<LayerConfig>();
        }

        public override void OnInspectorGUI()
        {
            
            
            if (layerConfig == null)
            {
                EditorGUILayout.HelpBox("You do not have a LayerConfig set! Please create that first", MessageType.Error);
                return;
            }
            
            
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "enabledEvents");

            
            EditorGUILayout.LabelField("Collision Events: ");
            EditorGUILayout.BeginHorizontal();
            MarbleCollisionHandler.Events newEvents = 0;
            MarbleCollisionHandler.Events clickedThisFrame = 0;
            ShowToggle(MarbleCollisionHandler.Events.Enter);
            ShowToggle(MarbleCollisionHandler.Events.Stay);
            ShowToggle(MarbleCollisionHandler.Events.Exit);

            if (SingleEvent && clickedThisFrame != 0)
            {
                newEvents = clickedThisFrame;
            }
            
            if (newEvents != collisionHandler.EnabledEvents)
            {
                Undo.RecordObject(collisionHandler, "Updated events");
                collisionHandler.EnabledEvents = newEvents;
            }

            void ShowToggle(MarbleCollisionHandler.Events eventToShow)
            {
                if (RestrictedEvents.HasFlag(eventToShow))
                {
                    bool isEnabled = collisionHandler.EnabledEvents.HasFlag(eventToShow);
                    if (GUILayout.Toggle(isEnabled, eventToShow.ToString() + (isEnabled ? "✓" : ""), "Button"))
                    {
                        if (!isEnabled)
                        {
                            clickedThisFrame = eventToShow;
                        }
                        newEvents |= eventToShow;
                    }
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            errors.Clear();
            errors.AppendLine("Errors found!:");
            
            bool hasErrors = false;

            if (!collisionHandler.TryGetComponent(out Collider2D collider2D))
            {
                errors.AppendLine("- A collision handler requires a Collider2D!");
                hasErrors = true;
            }
            else if (RequireTrigger && !collider2D.isTrigger)
            {
                errors.AppendLine("- This component requires the collider to be a trigger!");
                hasErrors = true;
            }
            
            if (collisionHandler.gameObject.layer != lastLayer)
            {
                lastLayer = collisionHandler.gameObject.layer;

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

            if (collisionHandler.EnabledEvents == 0)
            {
                hasErrors = true;
                errors.AppendLine("- This will do nothing if you dont have any events enabled!");
            }

            if (hasCollisionError)
            {
                hasErrors = true;
                errors.AppendLine("- The layer this game object is on can not collide with any of the defined marble layers in your MarbleCollisionHandler!");
            }

            bool inheritedErrors = TryAppendErrors(errors);
            hasErrors |= inheritedErrors;

            if (hasErrors)
            {
                EditorGUILayout.HelpBox(errors.ToString().TrimEnd(), MessageType.Error);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual bool TryAppendErrors(StringBuilder stringBuilder)
        {
            return false;
        }


    }
}
