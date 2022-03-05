using UnityEngine;

namespace MarblePhysics
{
    public class PhysicsPositionOscillator : MonoBehaviour
    {
        [SerializeField]
        private FloatGenerator floatGenerator = default;

        [SerializeField]
        private Rigidbody2D targetRigidBody = default;

        [SerializeField]
        private Transform pointA = default;

        [SerializeField]
        private Transform pointB = default;


        private void FixedUpdate()
        {
            targetRigidBody.MovePosition(GetNextPosition());
        }
        
        public Vector3 GetNextPosition()
        {
            return Vector3.Lerp(pointA.position, pointB.position, floatGenerator.GetFixedValue());
        }
    }
}