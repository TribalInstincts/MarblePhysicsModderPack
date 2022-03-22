using MarblePhysics.Modding.Shared.Player;
using MarblePhysics.Modding;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public class LinearGravityTrigger : MarbleCollisionHandler
    {
        [SerializeField]
        private AngleVector angleVector = new() {Angle = -90f, Magnitude = 9.81f};
        public AngleVector AngleVector => angleVector;

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
            Vector2 force = angleVector.GetVector(this.transform);
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

    }
}