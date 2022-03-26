using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [CustomEditor(typeof(CutVolume))]
    public class CutVolumeEditor : Editor
    {
        private CutVolume cutVolume;
        private EnvironmentCanvas[] canvases;
        private Collider2D collider2D;

        private void OnEnable()
        {
            cutVolume = target as CutVolume;
            canvases = FindObjectsOfType<EnvironmentCanvas>();
            collider2D = cutVolume.gameObject.GetComponent<Collider2D>();
        }

        private void OnSceneGUI()
        {
            foreach (EnvironmentCanvas environmentCanvas in canvases)
            {
                environmentCanvas.Init();
                Bounds bounds = environmentCanvas.Bounds;
                if (Vector2.SqrMagnitude(cutVolume.transform.position - bounds.center) <= Vector2.SqrMagnitude(bounds.extents))
                {
                    environmentCanvas.CutHoles();
                }
            }
        }
    }
}
