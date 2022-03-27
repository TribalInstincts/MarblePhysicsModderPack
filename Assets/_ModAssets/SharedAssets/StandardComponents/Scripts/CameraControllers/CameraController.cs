using MarblePhysics.Modding.Shared;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public abstract class CameraController : MonoBehaviour
    {
        public abstract (Vector2 position, float size, Bounds2D?) GetCameraPlacement(Camera forCamera);
    }
}
