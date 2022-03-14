using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public class RadialGravityTrigger : MarbleCollisionHandler
    {
        [SerializeField, Tooltip("The strength of the gravity. Negative numbers pull towards the center. Positive numbers repulse.")]
        private float magnitude = -9.81f;

        [SerializeField]
        private EffectorForceMode2D forceMode = default;

        [SerializeField]
        private float gravityDistanceScale = 1;

        [SerializeField, Tooltip("When this is enabled, the velocity of the object will instantly change.")]
        private bool instantChange = default;

        [SerializeField, Tooltip("When this is true, a force is applied only while inside the collider. " +
                                 "If it is false, the marbles gravity is permanently changed, until something else changes it.")]
        private bool isTemporary = false;

        protected override void OnMarbleTriggerEnter(Marble marble)
        {
            ApplyGravity(marble);
        }

        protected override void OnMarbleTriggerStay(Marble marble)
        {
            ApplyGravity(marble);
        }

        protected override void OnMarbleCollisionEnter(Marble marble, Collision2D other)
        {
            ApplyGravity(marble);
        }

        protected override void OnMarbleCollisionStay(Marble marble, Collision2D other)
        {
            ApplyGravity(marble);
        }

        private void ApplyGravity(Marble marble)
        {
            Vector2 force = GetGravity(marble.transform.position);
            if (isTemporary)
            {
                if (instantChange)
                {
                    marble.SetVelocity(force);
                }
                else
                {
                    marble.ApplyForce(force);
                }
            }
            else
            {
                marble.SetGravity(force, instantChange);
            }
        }

        private Vector2 GetGravity(Vector3 otherPosition)
        {
            Vector2 direction = (otherPosition - transform.position) * gravityDistanceScale;
            Vector2 normalizedDirection = direction.normalized;
            switch (forceMode)
            {
                case EffectorForceMode2D.InverseLinear:
                    return normalizedDirection * (magnitude / direction.magnitude);
                case EffectorForceMode2D.InverseSquared:
                    return normalizedDirection * (magnitude / direction.sqrMagnitude);
                default:
                    return normalizedDirection * magnitude;
            }
        }
    }
}