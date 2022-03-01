using UnityEngine;

namespace MarblePhysics
{
    public class PhysicsRotator : MonoBehaviour
    {
        [SerializeField]
        private FloatGenerator floatGenerator = default;

        [SerializeField]
        private Rigidbody2D rigidbody2D = default;

        [SerializeField]
        private Space angleSpace = default;

        private void FixedUpdate()
        {
            rigidbody2D.MoveRotation(ConvertSpace(floatGenerator.GetFixedValue()));
        }

        public float GetStartingAngle()
        {
            return ConvertSpace(floatGenerator.GetMinimum());
        }
        
        public float GetEndingAngle()
        {
            return ConvertSpace(floatGenerator.GetMaximum());
        }
        
        private float ConvertSpace(float angle)
        {
            if (angleSpace == Space.Self)
            {
                angle += transform.eulerAngles.z;
            }

            return angle;
        }
    }
}