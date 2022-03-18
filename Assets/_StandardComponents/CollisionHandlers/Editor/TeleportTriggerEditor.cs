using UnityEditor;

namespace MarblePhysics.Modding.StandardComponents
{
    [CustomEditor(typeof(TeleportTrigger))]
    public class TeleportTriggerEditor : MarbleCollisionHandlerEditor
    {
        private TeleportTrigger trigger;
        private MarbleCollisionHandler.Events currentEvent;

        protected override void OnEnable()
        {
            trigger = target as TeleportTrigger;

            trigger.EnabledEvents = trigger.EnabledEvents.HasFlag(MarbleCollisionHandler.Events.Enter) ? MarbleCollisionHandler.Events.Enter : MarbleCollisionHandler.Events.Exit;
            currentEvent = trigger.EnabledEvents;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (trigger.EnabledEvents != currentEvent)
            {
                // if stay is enabled - turn it off.
                if (trigger.EnabledEvents.HasFlag(MarbleCollisionHandler.Events.Stay))
                {
                    trigger.EnabledEvents = currentEvent;
                }
                else
                {
                    Undo.RecordObject(trigger, "Toggled events");
                    // else - toggle between the two.
                    if (currentEvent == MarbleCollisionHandler.Events.Enter)
                    {
                        trigger.EnabledEvents = MarbleCollisionHandler.Events.Exit;
                    }
                    else
                    {
                        trigger.EnabledEvents = MarbleCollisionHandler.Events.Enter;
                    }
                }
            }
            
        }
    }
}