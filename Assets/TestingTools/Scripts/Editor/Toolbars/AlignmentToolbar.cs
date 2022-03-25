using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace MarblePhysics.Modding.Test
{
    [Overlay(typeof(SceneView), "MarblePhysics/Alignment Toolbar")]
    [Icon(EditorIcon.Paths.icon_align_bottom)]
    public class AlignmentToolbar : ToolbarOverlay
    {
        public static Action<bool> CanAlignChanged;

        private const string toolbar = "AlignmentToolbar";
        public const string AlignLeft = toolbar + "/Align Left";
        public const string CenterVertical = toolbar + "/Align Center Vertical";
        public const string AlignRight = toolbar + "/Align Right";
        public const string AlignTop = toolbar + "/Align Top";
        public const string CenterHorizontal = toolbar + "/Align Center Horizontal";
        public const string AlignBottom = toolbar + "/Align Bottom";

        AlignmentToolbar() : base(
            AlignLeft,
            CenterVertical,
            AlignRight,
            AlignTop,
            CenterHorizontal,
            AlignBottom
        )
        {
        }

        private bool canAlign = false;

        public override void OnCreated()
        {
            base.OnCreated();
            Selection.selectionChanged += OnSelectionChanged;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            EditorAlignmentUtil.OnSelectionChanged();
            bool canAlign = EditorAlignmentUtil.CanAlignToFocusObject();
            if (canAlign != this.canAlign)
            {
                this.canAlign = canAlign;
                CanAlignChanged?.Invoke(canAlign);
            }
        }
    }
    
    abstract class AlignmentButton : EditorToolbarButton
    {
        protected abstract string IconPath { get; }
        protected virtual string ToolTip { get; }
        protected abstract void OnClick();

        protected AlignmentButton() : base()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
            tooltip = ToolTip;
            clicked += OnClick;
            SetEnabled(EditorAlignmentUtil.CanAlignToFocusObject());
            AlignmentToolbar.CanAlignChanged += CanAlignChanged;
        }

        private void CanAlignChanged(bool canAlign)
        {
            SetEnabled(canAlign);
        }
    }

    [EditorToolbarElement(AlignmentToolbar.AlignLeft, typeof(SceneView))]
    class AlignLeft : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_left;
        protected override void OnClick() => EditorAlignmentUtil.AlignEdges(Vector2.left);
    }

    [EditorToolbarElement(AlignmentToolbar.CenterVertical, typeof(SceneView))]
    class CenterVertical : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_center_vertical;
        protected override void OnClick() => EditorAlignmentUtil.Center(false);
    }

    [EditorToolbarElement(AlignmentToolbar.AlignRight, typeof(SceneView))]
    class AlignRight : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_right;
        protected override void OnClick() => EditorAlignmentUtil.AlignEdges(Vector2.right);
    }

    [EditorToolbarElement(AlignmentToolbar.AlignTop, typeof(SceneView))]
    class AlignTop : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_top;
        protected override void OnClick() => EditorAlignmentUtil.AlignEdges(Vector2.up);
    }

    [EditorToolbarElement(AlignmentToolbar.CenterHorizontal, typeof(SceneView))]
    class CenterHorizontal : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_center_horizontal;
        protected override void OnClick() => EditorAlignmentUtil.Center(true);
    }

    [EditorToolbarElement(AlignmentToolbar.AlignBottom, typeof(SceneView))]
    class AlignBottom : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_bottom;
        protected override void OnClick() => EditorAlignmentUtil.AlignEdges(Vector2.down);
    }
}