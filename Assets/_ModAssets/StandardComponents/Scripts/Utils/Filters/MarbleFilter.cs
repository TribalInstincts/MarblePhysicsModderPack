using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public abstract class MarbleFilter : MonoBehaviour
    {
        public abstract bool PassesFilter(Marble marble);
    }
}
