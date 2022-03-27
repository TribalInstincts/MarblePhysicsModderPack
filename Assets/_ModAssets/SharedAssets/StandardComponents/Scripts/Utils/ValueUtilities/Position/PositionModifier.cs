using UnityEngine;

namespace MarblePhysics.Modding
{
    public abstract class PositionModifier : MonoBehaviour
    {
        public abstract Vector2 ModifyPosition(Vector2 position);
    }
}
