using MarblePhysics.Modding.Shared;
using UnityEngine;

namespace MarblePhysics
{
    public interface ICameraController
    {
        public (Vector2 position, float size, Bounds2D?) GetCameraPlacement(Camera forCamera);
    }
}
