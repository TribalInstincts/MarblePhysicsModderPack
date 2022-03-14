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

        private void OnSceneGUI()
        {
            CheckSpaceChange();
            AngleVector angleVector = trigger.AngleVector;
            VectorHandle.DrawHandle(trigger.gameObject, angleVector.GetAngle, angleVector.SetAngle, angleVector.GetStrength, angleVector.SetMagnitude, angleVector.Space);
        }

        private void CheckSpaceChange()
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
            }
        }
    }
}