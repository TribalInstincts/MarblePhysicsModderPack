using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MarblePhysics.Modding
{
    public class RandomPositionModifier : PositionModifier
    {
        [Serializable]
        private enum Method
        {
            Circle
        }

        [SerializeField]
        private Method method = default;

        [SerializeField]
        private float sizeModifier = default;
        
        public override Vector2 ModifyPosition(Vector2 position)
        {
            switch (method)
            {
                case Method.Circle:
                    return position + (Random.insideUnitCircle * sizeModifier);
                default:
                    Debug.LogError("No method defined for: " + method);
                    return position;
            }
        }
    }
}
