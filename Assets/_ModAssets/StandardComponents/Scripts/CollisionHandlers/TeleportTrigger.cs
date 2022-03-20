using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public class TeleportTrigger : MarbleCollisionHandler
    {
        [SerializeField]
        private Transform teleportTarget = default;

        [SerializeField]
        private PositionModifier positionModifier = default;

        public Transform TeleportTarget => teleportTarget;

        [SerializeField]
        private bool keepVelocity = false;

        [SerializeField]
        private bool clearTrail = false;

        private bool hasPositionModifier = false;

        protected override void Awake()
        {
            base.Awake();
            hasPositionModifier = positionModifier != null;
        }

        protected override void OnMarbleTriggerEnter(Marble marble)
        {
            Teleport(marble);
        }

        protected override void OnMarbleTriggerExit(Marble marble)
        {
            Teleport(marble);
        }

        protected override void OnMarbleCollisionEnter(Marble marble, Collision2D other)
        {
            Teleport(marble);
        }

        protected override void OnMarbleCollisionExit(Marble marble, Collision2D other)
        {
            Teleport(marble);
        }

        private void Teleport(Marble marble)
        {
            marble.Teleport((hasPositionModifier ? positionModifier.ModifyPosition(teleportTarget.position) : teleportTarget.position), keepVelocity, true, clearTrail);
        }
    }
}