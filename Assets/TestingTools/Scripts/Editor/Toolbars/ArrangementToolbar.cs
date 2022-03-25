using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace MarblePhysics.Modding.Test
{
    [Overlay(typeof(SceneView), "MarblePhysics/Arrangement Options")]
    [Icon(EditorIcon.Paths.icon_align_spacing)]
    public class ArrangementToolbar : Overlay
    {
        private float spacingOffset = 1f;
        private EditorAlignmentUtil.Direction spacingDirection = EditorAlignmentUtil.Direction.Right;
        private EditorAlignmentUtil.Axis distributeAxis = EditorAlignmentUtil.Axis.Horizontal;
        private EditorAlignmentUtil.DistributeMethod distributeMethod = EditorAlignmentUtil.DistributeMethod.Bounds;

        private bool showSpacing = true;
        private bool showDistribution = true;

        public override VisualElement CreatePanelContent()
        {
            return new IMGUIContainer(DrawGUI);
        }
        
        private void DrawGUI()
        {
            showSpacing = EditorGUILayout.Foldout(showSpacing, "Even Spacing");
            if (showSpacing)
            {
                EditorGUI.BeginChangeCheck();
                spacingDirection = (EditorAlignmentUtil.Direction) EditorGUILayout.EnumPopup("Direction:", spacingDirection);
                spacingOffset = EditorGUILayout.DelayedFloatField("Spacing Offset:", spacingOffset);

                GUI.enabled = EditorAlignmentUtil.CanAlign();
                if (GUILayout.Button("Arrange") || EditorGUI.EndChangeCheck())
                {
                    EditorAlignmentUtil.UpdateSpacing(spacingDirection, spacingOffset);
                }

                GUI.enabled = true;
            }

            showDistribution = EditorGUILayout.Foldout(showDistribution, "Even Distribution");
            if (showDistribution)
            {
                distributeMethod = (EditorAlignmentUtil.DistributeMethod) EditorGUILayout.EnumPopup("Distribute Method:", distributeMethod);
                distributeAxis = (EditorAlignmentUtil.Axis) EditorGUILayout.EnumPopup("Axis:", distributeAxis);
                GUI.enabled = EditorAlignmentUtil.CanDistribute();
                if (GUILayout.Button("Distribute"))
                {
                    EditorAlignmentUtil.Distribute(distributeMethod, distributeAxis);
                }

                GUI.enabled = true;
            }
        }
    }
}