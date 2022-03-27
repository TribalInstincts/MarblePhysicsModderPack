using System;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    /// <summary>
    /// Convenience class to avoid some boiler plate getComponent checks. Works for 
    /// </summary>
    public class MarbleCollisionHandler : MonoBehaviour
    {
        [Serializable, Flags]
        public enum Events
        {
            Enter = 1,
            Exit = 2,
            Stay = 4
        }

        [SerializeField]
        private Events enabledEvents = Events.Enter | Events.Exit;
        public Events EnabledEvents
        {
            get => enabledEvents;
            set => enabledEvents = value;
        }

        [SerializeField]
        private MarbleFilter filter = default;
        
        private bool usesFilter;

        protected virtual void Awake()
        {
            usesFilter = filter != null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (enabledEvents.HasFlag(Events.Enter) && other.TryGetComponent(out Marble marble) && PassesFilter(marble))
            {
                OnMarbleTriggerEnter(marble);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (enabledEvents.HasFlag(Events.Exit) && other.TryGetComponent(out Marble marble) && PassesFilter(marble))
            {
                OnMarbleTriggerExit(marble);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (enabledEvents.HasFlag(Events.Stay) && other.TryGetComponent(out Marble marble) && PassesFilter(marble))
            {
                OnMarbleTriggerStay(marble);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (enabledEvents.HasFlag(Events.Enter) && other.gameObject.TryGetComponent(out Marble marble) && PassesFilter(marble))
            {
                OnMarbleCollisionEnter(marble, other);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (enabledEvents.HasFlag(Events.Exit) && other.gameObject.TryGetComponent(out Marble marble) && PassesFilter(marble))
            {
                OnMarbleCollisionExit(marble, other);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (enabledEvents.HasFlag(Events.Stay) && other.gameObject.TryGetComponent(out Marble marble) && PassesFilter(marble))
            {
                OnMarbleCollisionStay(marble, other);
            }
        }
        
        private bool PassesFilter(Marble marble)
        {
            return !usesFilter || filter.PassesFilter(marble);
        }

        /// <summary>
        /// Called once on the frame a marble touches a collider that is set to IsTrigger.
        /// </summary>
        protected virtual void OnMarbleTriggerEnter(Marble marble)
        {
        }

        /// <summary>
        /// Called once on the frame a marble is no longer touching a collider that is set to IsTrigger.
        /// </summary>
        protected virtual void OnMarbleTriggerExit(Marble marble)
        {
        }

        /// <summary>
        /// Called each frame while a marble is touching a collider that is set to IsTrigger.
        /// </summary>
        protected virtual void OnMarbleTriggerStay(Marble marble)
        {
        }

        /// <summary>
        /// Called once on the frame a marble hits a collider
        /// </summary>
        protected virtual void OnMarbleCollisionEnter(Marble marble, Collision2D other)
        {
        }


        /// <summary>
        /// Called once on the frame a marble leaves contact with a collider
        /// </summary>
        protected virtual void OnMarbleCollisionExit(Marble marble, Collision2D other)
        {
        }

        /// <summary>
        /// Called each frame while a marble is touching a collider
        /// </summary>
        protected virtual void OnMarbleCollisionStay(Marble marble, Collision2D other)
        {
        }
    }
}