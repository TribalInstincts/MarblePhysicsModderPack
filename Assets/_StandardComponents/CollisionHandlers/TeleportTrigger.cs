using System.Collections;
using System.Collections.Generic;
using MarblePhysics.Modding.Shared.Player;
using MarblePhysics.Modding.StandardComponents;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public class TeleportTrigger : MarbleCollisionHandler
    {
        [SerializeField]
        private Transform teleportTarget = default;

        [SerializeField]
        private bool keepVelocity = false;

        [SerializeField]
        private bool clearTrail = false;

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
            marble.Teleport(teleportTarget.position, keepVelocity, true, clearTrail);
        }
    }
}