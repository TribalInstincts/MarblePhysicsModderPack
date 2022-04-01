using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public class TeleportTrigger : MarbleCollisionHandler
    {
        [SerializeField]
        private Transform teleportTarget = default;

        public Transform TeleportTarget => teleportTarget;

        private bool hasTeleportHandler = false;
        private ITeleportHandler teleportHandler; 

        protected override void Awake()
        {
            base.Awake();
            hasTeleportHandler = teleportTarget.TryGetComponent(out teleportHandler);
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
            if (hasTeleportHandler)
            {
                teleportHandler.HandleTeleport(marble);
            }
            else
            {
                marble.Teleport(teleportTarget.position, false, true, false);
            }
            
        }
    }
}