using UnityEngine;

namespace MarblePhysics.Modding
{
    public class PhysicsRotator : MonoBehaviour
    {
        [SerializeField]
        private FloatGenerator floatGenerator = default;

        [SerializeField]
        private Rigidbody2D targetRigidBody = default;

        [SerializeField]
        private Space angleSpace = default;

        private void FixedUpdate()
        {
            targetRigidBody.MoveRotation(ConvertSpace(floatGenerator.GetFixedValue()));
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