using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    /// <summary>
    /// Convenience class to avoid some boiler plate getComponent checks. Works for 
    /// </summary>
    public class MarbleCollisionHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("OnStay events are disabled by default due to them being less commonly used and " +
                                 "have a(VERY SMALL) per-frame performance cost. Don't hesitate to enable if you need it")]
        private bool usesStayEvents = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Marble marble))
            {
                OnMarbleTriggerEnter(marble);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Marble marble))
            {
                OnMarbleTriggerExit(marble);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (usesStayEvents && other.TryGetComponent(out Marble marble))
            {
                OnMarbleTriggerStay(marble);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Marble marble))
            {
                OnMarbleCollisionEnter(marble, other);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Marble marble))
            {
                OnMarbleCollisionExit(marble, other);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (usesStayEvents && other.gameObject.TryGetComponent(out Marble marble))
            {
                OnMarbleCollisionStay(marble, other);
            }
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
        private void OnMarbleTriggerExit(Marble other)
        {
        }

        /// <summary>
        /// Called each frame while a marble is touching a collider that is set to IsTrigger.
        /// </summary>
        private void OnMarbleTriggerStay(Marble other)
        {
        }

        /// <summary>
        /// Called once on the frame a marble hits a collider
        /// </summary>
        private void OnMarbleCollisionEnter(Marble marble, Collision2D other)
        {
        }


        /// <summary>
        /// Called once on the frame a marble leaves contact with a collider
        /// </summary>
        private void OnMarbleCollisionExit(Marble marble, Collision2D other)
        {
        }

        /// <summary>
        /// Called each frame while a marble is touching a collider
        /// </summary>
        private void OnMarbleCollisionStay(Marble marble, Collision2D other)
        {
        }
    }
}