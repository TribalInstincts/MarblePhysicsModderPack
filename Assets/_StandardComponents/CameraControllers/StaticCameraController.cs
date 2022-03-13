using MarblePhysics.Modding.Shared;
using UnityEngine;

namespace MarblePhysics
{
    public class StaticCameraController : ICameraController
    {
        public (Vector2 position, float size, Bounds2D?) GetCameraPlacement(Camera forCamera)
        {
            return (Vector2.zero, 22, null);
        }
    }
}
