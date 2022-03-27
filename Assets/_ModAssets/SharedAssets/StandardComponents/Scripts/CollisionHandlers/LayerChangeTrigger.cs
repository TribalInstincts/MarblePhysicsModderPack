using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public class LayerChangeTrigger : MarbleCollisionHandler
    {
        [SerializeField, Layer]
        private int toLayer = default;

        protected override void OnMarbleTriggerEnter(Marble marble)
        {
            marble.gameObject.layer = toLayer;
        }
    }
}
