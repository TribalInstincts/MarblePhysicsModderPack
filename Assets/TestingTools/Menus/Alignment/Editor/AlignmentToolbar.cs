using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [Overlay(typeof(SceneView), "MarblePhysics Alignment Toolbar")]
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
            AlignmentUtil.OnSelectionChanged();
            bool canAlign = AlignmentUtil.CanAlign();
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
            tooltip = "Instantiate a cube in the scene.";
            clicked += OnClick;
            SetEnabled(AlignmentUtil.CanAlign());
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
        protected override void OnClick() => AlignmentUtil.AlignEdges(Vector2.left);
    }

    [EditorToolbarElement(AlignmentToolbar.CenterVertical, typeof(SceneView))]
    class CenterVertical : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_center_vertical;
        protected override void OnClick() => AlignmentUtil.Center(false);
    }

    [EditorToolbarElement(AlignmentToolbar.AlignRight, typeof(SceneView))]
    class AlignRight : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_right;
        protected override void OnClick() => AlignmentUtil.AlignEdges(Vector2.right);
    }

    [EditorToolbarElement(AlignmentToolbar.AlignTop, typeof(SceneView))]
    class AlignTop : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_top;
        protected override void OnClick() => AlignmentUtil.AlignEdges(Vector2.up);
    }

    [EditorToolbarElement(AlignmentToolbar.CenterHorizontal, typeof(SceneView))]
    class CenterHorizontal : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_center_horizontal;
        protected override void OnClick() => AlignmentUtil.Center(true);
    }

    [EditorToolbarElement(AlignmentToolbar.AlignBottom, typeof(SceneView))]
    class AlignBottom : AlignmentButton
    {
        protected override string IconPath => EditorIcon.Paths.icon_align_bottom;
        protected override void OnClick() => AlignmentUtil.AlignEdges(Vector2.down);
    }


    public static class AlignmentUtil
    {
        private static GameObject alignmentTarget = null;

        public static bool CanAlign()
        {
            return Selection.count > 1 && alignmentTarget != null;
        }

        public static void OnSelectionChanged()
        {
            if (Selection.count == 1)
            {
                if (Selection.activeGameObject != null)
                {
                    alignmentTarget = Selection.activeGameObject;
                }
                else
                {
                    alignmentTarget = null;
                }
            }
            else if (Selection.count == 0)
            {
                alignmentTarget = null;
            }
        }

        public static void AlignEdges(Vector2 anchor)
        {
            Undo.RecordObjects(Selection.objects, "Alignment Event");
            Vector2 alignmentPoint = GetBoundsPosition(alignmentTarget, anchor);
            Vector2 absAnchor = new Vector2(Mathf.Abs(anchor.x), Mathf.Abs(anchor.y));
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                if (gameObject != alignmentTarget)
                {
                    Vector2 otherAlignmentPoint = GetBoundsPosition(gameObject, anchor);
                    Vector2 adjustDirection = alignmentPoint - otherAlignmentPoint;
                    adjustDirection *= absAnchor;
                    gameObject.transform.position += (Vector3) adjustDirection;
                }
            }
        }

        public static void Center(bool horizontal)
        {
            Undo.RecordObjects(Selection.objects, "Alignment Event");
            Vector2 targetMult = horizontal ? new Vector2(0, 1) : new Vector2(1, 0);
            Vector2 otherMult = horizontal ? new Vector2(1, 0) : new Vector2(0, 1);

            Vector2 alignmentPoint = GetBoundsPosition(alignmentTarget) * targetMult;

            foreach (GameObject gameObject in Selection.gameObjects)
            {
                if (gameObject != alignmentTarget)
                {
                    Vector2 otherAlignmentPoint = GetBoundsPosition(gameObject) * otherMult;
                    gameObject.transform.position = alignmentPoint + otherAlignmentPoint;
                }
            }
        }


        private static Vector2 GetBoundsPosition(GameObject gameObject)
        {
            return GetBoundsPosition(gameObject, Vector2.zero);
        }

        private static Vector2 GetBoundsPosition(GameObject gameObject, Vector2 anchor)
        {
            if (gameObject.TryGetComponent(out Collider2D collider))
            {
                Bounds colliderBounds = collider.bounds;
                Vector2 center = colliderBounds.center;
                Vector2 extent = colliderBounds.extents * anchor;
                Vector2 worldPosition = center + extent;
                return worldPosition;
            }
            else
            {
                return gameObject.transform.position;
            }
        }
    }
}