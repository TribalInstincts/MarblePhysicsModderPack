using UnityEngine;

namespace MarblePhysics.Modding
{
    public class PhysicsPositionOscillator : MonoBehaviour
    {
        [SerializeField]
        private FloatGenerator floatGenerator = default;

        [SerializeField]
        private Rigidbody2D targetRigidBody = default;

        public Rigidbody2D TargetRigidBody => targetRigidBody;

        [SerializeField]
        private Transform pointA = default;
        public Transform PointA
        {
            get => pointA;
            set => pointA = value;
        }

        [SerializeField]
        private Transform pointB = default;
        public Transform PointB
        {
            get => pointB;
            set => pointB = value;
        }


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