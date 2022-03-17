using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    [CustomEditor(typeof(LinearGravityTrigger))]
    [CanEditMultipleObjects]
    public class LinearGravityTriggerEditor : MarbleCollisionHandlerEditor
    {
        private LinearGravityTrigger trigger;
        
        private Space lastSpace;
        protected override void OnEnable()
        {
            base.OnEnable();
            trigger = target as LinearGravityTrigger;
            lastSpace = trigger.AngleVector.Space;
        }

        protected void OnSceneGUI()
        {
            bool updated = CheckSpaceChange();
            AngleVector angleVector = trigger.AngleVector;
            updated |= VectorHandle.DrawHandle(trigger.gameObject, trigger, angleVector);
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

                trigger.AngleVector.Angle = angle;

                lastSpace = currentSpace;
                return true;
            }

            return false;
        }
    }
}