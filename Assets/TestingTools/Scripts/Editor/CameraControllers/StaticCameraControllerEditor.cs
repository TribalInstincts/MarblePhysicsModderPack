using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding.Test
{
    [CustomEditor(typeof(StaticCameraController))]
    public class StaticCameraControllerEditor : Editor
    {
        private void OnSceneGUI()
        {
            StaticCameraController controller = target as StaticCameraController;
            Vector2 center = controller.transform.position;
            float halfSize = controller.Size;
            Vector2 vert = new Vector2(0, halfSize);
            Vector2 hori = new Vector2(halfSize * (16f / 9f), 0);

            Handles.DrawPolyLine((center + vert - hori), (center + vert + hori), (center - vert + hori), (center - vert - hori), (center + vert - hori));
        }
    }
}
