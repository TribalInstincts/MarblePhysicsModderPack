using UnityEngine;

namespace MarblePhysics.Modding
{
    public class DoNotRotate : MonoBehaviour
    {
        [SerializeField]
        private bool keepRelativePositionToParent = true;

        [SerializeField]
        private Rigidbody2D rb = default;

        private float rotationAtStart;
        private Vector3 positionOffsetFromParent;
        private bool useRigidBody = false;

        private void Awake()
        {
            useRigidBody = rb != null;
            if (useRigidBody)
            {
                rotationAtStart = rb.rotation;
            }
            else
            {
                rotationAtStart = transform.eulerAngles.z;
            }
            positionOffsetFromParent = transform.position - transform.parent.position;
        }

        private void LateUpdate()
        {
            if (useRigidBody)
            {
                rb.SetRotation(rotationAtStart);
                if (keepRelativePositionToParent)
                {
                    rb.position = transform.parent.position + positionOffsetFromParent;
                } 
            }
            else
            {
                transform.rotation = Quaternion.identity;
                if (keepRelativePositionToParent)
                {
                    transform.position = transform.parent.position + positionOffsetFromParent;
                }    
            }
        
        }
    }
}