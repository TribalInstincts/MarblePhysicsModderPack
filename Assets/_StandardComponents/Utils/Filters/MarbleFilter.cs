using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents.Filters
{
    public abstract class MarbleFilter : MonoBehaviour
    {
        public abstract bool PassesFilter(Marble marble);
    }
}
