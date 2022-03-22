using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.Test
{
    [CustomEditor(typeof(RadialGravityTrigger))]
    [CanEditMultipleObjects]
    public class RadialGravityTriggerEditor : MarbleCollisionHandlerEditor
    {
        private RadialGravityTrigger trigger;
        
        private Space lastSpace;
        protected override void OnEnable()
        {
            base.OnEnable();
            trigger = target as RadialGravityTrigger;
        }

        protected void OnSceneGUI()
        {
            Vector3 position = trigger.transform.TransformPoint(trigger.GravityCenterLocalOffset);
            
            Vector2 newPosition = Handles.FreeMoveHandle(position, HandleUtility.GetHandleSize(position) * 0.1f, Vector3.zero, Handles.CircleHandleCap);
            Vector2 newOffset = trigger.transform.InverseTransformPoint(newPosition);

            if (Mathf.Abs((newOffset - trigger.GravityCenterLocalOffset).sqrMagnitude) > Mathf.Epsilon)
            {
                Undo.RecordObject(trigger, "Gravity Center Changed");
                trigger.GravityCenterLocalOffset = newOffset;
            }
        }

    }
}
