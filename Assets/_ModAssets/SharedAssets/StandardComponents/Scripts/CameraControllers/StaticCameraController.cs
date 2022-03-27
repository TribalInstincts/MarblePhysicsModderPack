using MarblePhysics.Modding.Shared;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public class StaticCameraController : CameraController
    {
        [SerializeField]
        private float size = default;

        public float Size
        {
            get => size;
            set => size = value;
        }
        
        public override (Vector2 position, float size, Bounds2D?) GetCameraPlacement(Camera forCamera)
        {
            return (transform.position, size, null);
        }
    }
}
