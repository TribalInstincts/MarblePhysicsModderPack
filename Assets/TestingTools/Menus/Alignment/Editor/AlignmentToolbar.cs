using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace MarblePhysics.Modding
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
            AlignmentUtil.OnSelectionChanged();
            bool canAlign = AlignmentUtil.CanAlignToFocusObject();
            if (canAlign != this.canAlign)
            {
                this.canAlign = canAlign;
                CanAlignChanged?.Invoke(canAlign);
            }
        }
    }


    #region AlignmentButtons

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
            SetEnabled(AlignmentUtil.CanAlignToFocusObject());
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

    #endregion


    #region SpacingButton

    [Overlay(typeof(SceneView), "MarblePhysics/Arrangement Options")]
    [Icon(EditorIcon.Paths.icon_align_spacing)]
    public class ArrangementPanel : Overlay
    {
        private float spacingOffset = 1f;
        private AlignmentUtil.Direction direction = AlignmentUtil.Direction.Right;
        private bool updateToggled = false;

        public ArrangementPanel()
        {
        }

        public override VisualElement CreatePanelContent()
        {
            return new IMGUIContainer(DrawGUI);
        }


        private void DrawGUI()
        {
            EditorGUI.BeginChangeCheck();
            direction = (AlignmentUtil.Direction) EditorGUILayout.EnumPopup("Direction:", direction);
            spacingOffset = EditorGUILayout.DelayedFloatField("Spacing Offset:", spacingOffset);

            if (AlignmentUtil.CanAlign())
            {
                updateToggled = GUILayout.Button("Arrange");
                if (EditorGUI.EndChangeCheck() || updateToggled)
                {
                    AlignmentUtil.UpdateSpacing(direction, spacingOffset);
                }
            }
            else
            {
                updateToggled = false;
            }
        }
    }

    #endregion


    public static class AlignmentUtil
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public static Vector2 AsVector(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right
            };
        }

        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left
            };
        }

        static AlignmentUtil()
        {
            SceneView.duringSceneGui += SceneViewOnduringSceneGui;
        }

        private static void SceneViewOnduringSceneGui(SceneView obj)
        {
            if (CanAlignToFocusObject())
            {
                float size = 1;
                if (TryGetBounds(alignmentTarget, out Bounds bounds))
                {
                    size = Mathf.Max(bounds.extents.x, bounds.extents.y);
                }
                Handles.DrawSelectionFrame(0, alignmentTarget.transform.position, Quaternion.identity, size + .25f, EventType.Repaint);
            }
        }

        private static GameObject alignmentTarget = null;

        public static bool HasModifiedCurrentTarget { get; private set; }
        private static GameObject[] objectOrder;

        public static bool CanAlignToFocusObject()
        {
            return Selection.count > 1 && alignmentTarget != null;
        }

        public static bool CanAlign()
        {
            return Selection.gameObjects is {Length: > 2};
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
            else if (alignmentTarget == null || !Selection.Contains(alignmentTarget))
            {
                alignmentTarget = Selection.activeGameObject;
            }

            HasModifiedCurrentTarget = false;
            objectOrder = null;
        }

        public static void UpdateSpacing(Direction direction, float distance)
        {
            RecordChange();


            Vector2 vector = direction.AsVector();
            Vector2 offset = vector * distance;
            Vector2 oppositeVector = direction.Opposite().AsVector();
            Vector2 absDirection = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));

            if (objectOrder == null)
            {
                objectOrder = Selection.gameObjects.OrderBy(go => Vector2.Dot(vector, GetBoundsPosition(go, oppositeVector))).ToArray();
            }

            Vector2 nextPoint = GetBoundsPosition(alignmentTarget, vector) + offset;
            foreach (GameObject gameObject in objectOrder)
            {
                if (gameObject != alignmentTarget)
                {
                    Vector2 goStart = GetBoundsPosition(gameObject, oppositeVector);
                    Vector2 goEnd = GetBoundsPosition(gameObject, vector);
                    gameObject.transform.position += (Vector3) ((nextPoint - goStart) * absDirection);

                    nextPoint += (goEnd - goStart) + offset;
                }
            }
        }

        public static void AlignEdges(Vector2 anchor)
        {
            RecordChange();
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
            RecordChange();
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

        private static void RecordChange()
        {
            HasModifiedCurrentTarget = true;
            Undo.RecordObjects(Selection.gameObjects.Select(go => go.transform).ToArray(), "Alignment Event");
        }


        private static Vector2 GetBoundsPosition(GameObject gameObject)
        {
            return GetBoundsPosition(gameObject, Vector2.zero);
        }

        private static Vector2 GetBoundsPosition(GameObject gameObject, Vector2 anchor)
        {
            if (TryGetBounds(gameObject, out Bounds bounds))
            {
                Vector2 center = bounds.center;
                Vector2 extent = bounds.extents * anchor;
                Vector2 worldPosition = center + extent;
                return worldPosition;
            }
            else
            {
                return gameObject.transform.position;
            }
        }

        private static bool TryGetBounds(GameObject gameObject, out Bounds bounds)
        {
            bounds = default;
            if (gameObject.TryGetComponent(out Collider2D collider))
            {
                bounds = collider.bounds;
            }
            else if (gameObject.TryGetComponent(out SpriteRenderer sr))
            {
                bounds = sr.bounds;
            }
            else if (gameObject.TryGetComponent(out SpriteShapeRenderer ssr))
            {
                bounds = ssr.bounds;
            }

            return bounds != default;
        }
    }
}