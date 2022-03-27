using MarblePhysics.Modding.Shared.Player;
using MarblePhysics.Modding;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public class ApplyForceTrigger : MarbleCollisionHandler
    {
        [SerializeField]
        private ForceMode2D forceMode = default;

        [SerializeField]
        private AngleVector angleVector = new() {Angle = 90f, Magnitude = 5f};
        public AngleVector AngleVector => angleVector;

        protected override void OnMarbleTriggerEnter(Marble marble)
        {
            ApplyForce(marble);
        }

        protected override void OnMarbleTriggerStay(Marble marble)
        {
            ApplyForce(marble);
        }
        
        protected override void OnMarbleCollisionEnter(Marble marble, Collision2D other)
        {
            ApplyForce(marble);
        }
        
        protected override void OnMarbleCollisionStay(Marble marble, Collision2D other)
        {
            ApplyForce(marble);
        }

        private void ApplyForce(Marble marble)
        {
            marble.ApplyForce(angleVector.GetVector(transform), forceMode);
        }
    }
}
