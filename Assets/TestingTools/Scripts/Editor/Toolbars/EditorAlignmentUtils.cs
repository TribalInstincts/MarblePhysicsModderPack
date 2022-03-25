using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace MarblePhysics.Modding.Test
{
    public static class EditorAlignmentUtil
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public enum Axis
        {
            Horizontal,
            Vertical
        }

        public static Vector2 AsVector(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public enum DistributeMethod
        {
            Bounds,
            Center
        }

        static EditorAlignmentUtil()
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
        private static GameObject[] spacingObjectOrder;

        public static bool CanAlignToFocusObject()
        {
            return Selection.count > 1 && alignmentTarget != null;
        }

        public static bool CanAlign()
        {
            return Selection.gameObjects is {Length: > 1};
        }

        public static bool CanDistribute()
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
            spacingObjectOrder = null;
        }

        public static void UpdateSpacing(Direction direction, float distance)
        {
            RecordChange();


            Vector2 vector = direction.AsVector();
            Vector2 offset = vector * distance;
            Vector2 oppositeVector = direction.Opposite().AsVector();
            Vector2 absDirection = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));

            if (spacingObjectOrder == null)
            {
                spacingObjectOrder = Selection.gameObjects.OrderBy(go => Vector2.Dot(vector, GetBoundsPosition(go, oppositeVector))).ToArray();
            }

            Vector2 nextPoint = GetBoundsPosition(alignmentTarget, vector) + offset;
            foreach (GameObject gameObject in spacingObjectOrder)
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

        public static void Distribute(DistributeMethod distributeMethod, Axis axis)
        {
            RecordChange();

            Vector2 direction = axis == Axis.Horizontal ? Vector2.right : Vector2.up;
            GameObject[] gos = Selection.gameObjects.OrderBy(go => Vector2.Dot(direction, go.transform.position)).ToArray();
            Vector2 first = gos[0].transform.position;
            Vector2 last = gos[^1].transform.position;
            if (distributeMethod == DistributeMethod.Bounds)
            {
                first = GetBoundsPosition(gos[0], direction);
                last = GetBoundsPosition(gos[^1], direction * -1f);
            }

            int remaining = gos.Length - 2;
            if (distributeMethod == DistributeMethod.Center)
            {
                float distanceBetween = Vector2.Distance(first, last);
                Vector2 offsetPer = direction * (distanceBetween / (remaining + 1));
                Vector2 nextPoint = first + offsetPer;
                for (int i = 1; i < gos.Length - 1; i++)
                {
                    GameObject gameObject = gos[i];

                    gameObject.transform.position += (Vector3) ((nextPoint - (Vector2) gameObject.transform.position) * direction);
                    nextPoint += offsetPer;
                }
            }
            else
            {
                float distanceBetween = Vector2.Distance(first * direction, last * direction);
                Vector2 availableSpace = direction * distanceBetween * direction;
                for (int i = 1; i < gos.Length - 1; i++)
                {
                    if (TryGetBounds(gos[i], out Bounds bounds))
                    {
                        availableSpace -= (Vector2) bounds.size;
                    }
                }

                Vector2 offsetPer = direction * (availableSpace / (remaining + 1));

                Vector2 nextPoint = first + offsetPer;
                for (int i = 1; i < gos.Length - 1; i++)
                {
                    GameObject gameObject = gos[i];

                    Vector2 size = Vector2.zero;
                    if (TryGetBounds(gos[i], out Bounds bounds))
                    {
                        size = (Vector2) bounds.size;
                    }

                    gameObject.transform.position += (Vector3) ((nextPoint - (Vector2) gameObject.transform.position + (size * .5f)) * direction);
                    nextPoint += offsetPer + (size * direction);
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