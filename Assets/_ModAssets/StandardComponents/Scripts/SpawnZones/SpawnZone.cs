using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public abstract class SpawnZone : MonoBehaviour
    {
        public abstract void PlaceMarbles(params Marble[] marbles);
    }
}