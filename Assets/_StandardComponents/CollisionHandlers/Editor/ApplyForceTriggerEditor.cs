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
            lastSpace = trigger.AngleVector.Space;
        }

        private void OnSceneGUI()
        {
            bool updated = CheckSpaceChange();
            AngleVector angleVector = trigger.AngleVector;
            updated |= VectorHandle.DrawHandle(trigger.gameObject, angleVector.GetAngle, angleVector.SetAngle, angleVector.GetStrength, angleVector.SetMagnitude, angleVector.Space);
            if (updated)
            {
                EditorUtility.SetDirty(trigger);
            }
        }

        private bool CheckSpaceChange()
        {
            Space currentSpace = trigger.AngleVector.Space;
            if (currentSpace != lastSpace)
            {
                float angle = trigger.AngleVector.Angle;
                if (currentSpace == Space.Self)
                {
                    angle -= trigger.transform.eulerAngles.z;
                }
                else
                {
                    angle += trigger.transform.eulerAngles.z;
                }
                
                trigger.AngleVector.SetAngle(angle);
                
                lastSpace = currentSpace;
                return true;
            }

            return false;
        }
    }
}