using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public abstract class SpawnZone : MonoBehaviour, ITeleportHandler
    {
        public abstract void PlaceMarbles(params Marble[] marbles);
        
        public virtual void HandleTeleport(Marble marble)
        {
            PlaceMarbles(marble);
        }
    }
}