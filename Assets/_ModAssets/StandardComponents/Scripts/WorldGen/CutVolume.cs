using System;
using System.Collections.Generic;
using ClipperLib;
using UnityEngine;
using static MarblePhysics.Modding.EnvironmentShaper;

namespace MarblePhysics.Modding
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CutVolume : MonoBehaviour
    {
        private delegate List<List<Vector2>> GetLocalPathsDelegate();

        public static HashSet<CutVolume> ActiveCutVolumes { get; private set; } = new HashSet<CutVolume>();

        [SerializeField]
        private bool isStaticShape = true;

        private GetLocalPathsDelegate getLocalPathsDelegate;

        private List<List<Vector2>> localPathCache;

        private void OnEnable()
        {
            ActiveCutVolumes.Add(this);
        }

        private void OnDisable()
        {
            ActiveCutVolumes.Remove(this);
        }

        public bool TryGetPaths(out List<List<IntPoint>> paths)
        {
            List<List<Vector2>> localPaths;
            if (isStaticShape && localPathCache != null)
            {
                localPaths = localPathCache;
            }
            else if (getLocalPathsDelegate != null || TryGetPointContainer(gameObject, out getLocalPathsDelegate))
            {
                localPaths = getLocalPathsDelegate();
            }
            else
            {
                paths = null;
                return false;
            }

            if (!isStaticShape)
            {
                localPathCache = localPaths;
            }

            paths = LocalPathsToWorldIntPaths(transform, localPaths);
            return true;
        }

        private static bool TryGetPointContainer(GameObject gameObject, out GetLocalPathsDelegate getLocalPathsDelegate)
        {
            if (gameObject.TryGetComponent(out PolygonCollider2D polyCollider))
            {
                getLocalPathsDelegate = () => GetLocalPaths(polyCollider);
                return true;
            }
            else if (gameObject.TryGetComponent(out BoxCollider2D boxCollider))
            {
                getLocalPathsDelegate = () => GetLocalPaths(boxCollider);
                return true;
            }
            else if (gameObject.TryGetComponent(out CircleCollider2D circleCollider))
            {
                getLocalPathsDelegate = () => GetLocalPaths(circleCollider);
                return true;
            }

            getLocalPathsDelegate = null;
            return false;
        }

        private static List<List<Vector2>> GetLocalPaths(BoxCollider2D collider2D)
        {
            Vector2 halfSize = collider2D.size * .5f;
            return new List<List<Vector2>>(){new()
            {
                new Vector2(-halfSize.x, -halfSize.y), // down left
                new Vector2(halfSize.x, -halfSize.y), // down right
                new Vector2(halfSize.x, halfSize.y), // up right
                new Vector2(-halfSize.x, halfSize.y), // up left
            }};
        }
        
        private static List<List<Vector2>> GetLocalPaths(CircleCollider2D collider2D)
        {
            int vertCount = (int)(((Vector2)collider2D.bounds.size).magnitude * 8);
            List<Vector2> points = new List<Vector2>(vertCount);

            float radians = 360.0f / vertCount * (float)Math.PI / 180.0f;
            for (int i = 0; i < vertCount; i++)
            {
                points.Add(new Vector2(collider2D.radius * Mathf.Cos(i * radians), collider2D.radius * Mathf.Sin(i * radians)));
            }
            
            return new List<List<Vector2>>(){points};
        }
        
        private static List<List<Vector2>> GetLocalPaths(PolygonCollider2D collider2D)
        {
            List<List<Vector2>> paths = new List<List<Vector2>>(collider2D.pathCount);
            for (int pathIndex = 0; pathIndex < collider2D.pathCount; pathIndex++)
            {
                paths.Add(new List<Vector2>(collider2D.GetPath(pathIndex)));
            }
            return paths;
        }
    }
}
