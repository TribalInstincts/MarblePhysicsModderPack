using System;
using System.Text;
using UnityEditor;

namespace MarblePhysics.Modding.StandardComponents
{
    [CustomEditor(typeof(TeleportTrigger))]
    public class TeleportTriggerEditor : MarbleCollisionHandlerEditor
    {
        private TeleportTrigger trigger;

        protected override void OnEnable()
        {
            base.OnEnable();
            base.SingleEvent = true;
            base.RestrictedEvents = MarbleCollisionHandler.Events.Enter | MarbleCollisionHandler.Events.Exit;
            trigger = target as TeleportTrigger;

        }

        protected override bool TryAppendErrors(StringBuilder stringBuilder)
        {
            bool hasErrors = false;
            if (trigger.TeleportTarget == null)
            {
                stringBuilder.AppendLine(" - must define a target to teleport to!");
                hasErrors = true;
            }

            return hasErrors;
        }

        private void OnSceneGUI()
        {
            if (trigger.TeleportTarget != null)
            {
                Handles.DrawDottedLine(trigger.transform.position, trigger.TeleportTarget.position, 2f);
            }
        }
    }
}