using MarblePhysics.Modding.Shared;
using UnityEngine;

namespace MarblePhysics
{
    public abstract class CameraController : MonoBehaviour
    {
        public abstract (Vector2 position, float size, Bounds2D?) GetCameraPlacement(Camera forCamera);
    }
}
