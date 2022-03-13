using System;
using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    [CustomEditor(typeof(ApplyForceTrigger))]
    [CanEditMultipleObjects]
    public class ApplyForceTriggerEditor : MarbleCollisionHandlerEditor
    {
        private ApplyForceTrigger trigger;

        private Space lastSpace;

        protected override void OnEnable()
        {
            base.OnEnable();
            trigger = target as ApplyForceTrigger;
            lastSpace = trigger.Space;
        }

        private void OnSceneGUI()
        {
            CheckSpaceChange();
            lastSpace = trigger.Space;
            Vector2 startPoint = trigger.transform.position;
            Vector2 direction = trigger.GetForceVector() * trigger.Strength;
            Vector2 endPoint = startPoint + direction;

            Handles.color = Color.white;
            Handles.DrawDottedLine(startPoint, endPoint, 2f);
            
            float size = HandleUtility.GetHandleSize(endPoint) * 0.1f;
            Handles.color = Color.red;
            EditorGUI.BeginChangeCheck();
            Vector3 updatedPosition = Handles.FreeMoveHandle(endPoint, size, Vector3.zero, Handles.CircleHandleCap);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Modified Trigger Force");
                
                Vector2 newDirection = (Vector2)updatedPosition - startPoint;
                float angle = Vector2.SignedAngle(Vector2.right, newDirection.normalized);
                if (trigger.Space == Space.Self)
                {
                    angle -= trigger.transform.eulerAngles.z;
                }
                
                trigger.SetForce(angle, newDirection.magnitude);
            }
        }

        private void CheckSpaceChange()
        {
            if (trigger.Space != lastSpace)
            {
                float angle = trigger.Angle;
                if (trigger.Space == Space.Self)
                {
                    angle -= trigger.transform.eulerAngles.z;
                }
                else
                {
                    angle += trigger.transform.eulerAngles.z;
                }
                
                trigger.SetAngle(angle);
            }
        }
    }
}