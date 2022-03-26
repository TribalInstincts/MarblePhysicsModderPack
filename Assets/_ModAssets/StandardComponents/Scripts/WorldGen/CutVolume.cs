using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using MarblePhysics.Modding.Shared.Theme;
using UnityEngine;
using UnityEngine.U2D;
using static MarblePhysics.Modding.EnvironmentCanvas;

namespace MarblePhysics.Modding
{
    [DisallowMultipleComponent]
    public class CutVolume : MonoBehaviour
    {
        private delegate List<List<Vector2>> GetLocalPathsDelegate();

        public bool IsStaticShape { get; private set; } = true;

        [SerializeField, Tooltip("Short story: When this is on, this cut volume will add to the environment, instead of subtract. Kind of... " +
                                 "Things get a little unpredictiable when you have multiple cuts on top of each other. Use at own risk." +
                                 "Longer story involves winding order of vertices and when this cuts boolean operation lands in the stack.")]
        private bool invert = default;
        public bool Invert
        {
            get => invert;
            set => invert = value;
        }
        
        private GetLocalPathsDelegate getLocalPathsDelegate;

        private List<List<Vector2>> localPathCache;
        private bool isInverted;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                if (!TryGetComponent(out ThemeColorSetter _))
                {
                    if (TryGetComponent(out SpriteRenderer sr))
                    {
                        sr.enabled = false;
                        Destroy(sr);
                    }
                    else if (TryGetComponent(out SpriteShapeRenderer ssr))
                    {
                        if (TryGetComponent(out SpriteShapeController ssc))
                        {
                            Destroy(ssc);
                        }

                        ssr.enabled = false;
                        Destroy(ssr);
                    }
                }
            }
        }

        public void ForcePathUpdate()
        {
            localPathCache = null;
        }

        public bool TryGetPaths(out List<List<IntPoint>> paths)
        {
            List<List<Vector2>> localPaths;
            if (IsStaticShape && Application.isPlaying && localPathCache != null)
            {
                if (isInverted != invert)
                {
                    isInverted = invert;
                    InvertPaths(ref localPathCache);
                }
                localPaths = localPathCache;
                
            }
            else if (getLocalPathsDelegate != null || TryGetPointContainer(gameObject, out getLocalPathsDelegate))
            {
                localPaths = getLocalPathsDelegate();
                if (invert)
                {
                    isInverted = invert;
                    InvertPaths(ref localPaths);
                }
            }
            else
            {
                paths = null;
                return false;
            }

            localPathCache = localPaths;

            paths = LocalPathsToWorldIntPaths(transform, localPaths);
            return true;
        }

        private void InvertPaths(ref List<List<Vector2>> paths)
        {
            foreach (List<Vector2> path in paths)
            {
                path.Reverse();
            }
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
            Vector2 offset = collider2D.offset;
            return new List<List<Vector2>>(){new()
            {
                offset + new Vector2(-halfSize.x, -halfSize.y), // down left
                offset + new Vector2(halfSize.x, -halfSize.y), // down right
                offset + new Vector2(halfSize.x, halfSize.y), // up right
                offset + new Vector2(-halfSize.x, halfSize.y), // up left
            }};
        }
        
        private static List<List<Vector2>> GetLocalPaths(CircleCollider2D collider2D)
        {
            int vertCount = (int)(((Vector2)collider2D.bounds.size).magnitude * 16);
            List<Vector2> points = new List<Vector2>(vertCount);
            Vector2 offset = collider2D.offset;

            float radians = 360.0f / vertCount * (float)Math.PI / 180.0f;
            for (int i = 0; i < vertCount; i++)
            {
                points.Add(offset + new Vector2(collider2D.radius * Mathf.Cos(i * radians), collider2D.radius * Mathf.Sin(i * radians)));
            }
            
            return new List<List<Vector2>>(){points};
        }
        
        private static List<List<Vector2>> GetLocalPaths(PolygonCollider2D collider2D)
        {
            Vector2 offset = collider2D.offset;
            List<List<Vector2>> paths = new List<List<Vector2>>(collider2D.pathCount);
            for (int pathIndex = 0; pathIndex < collider2D.pathCount; pathIndex++)
            {
                paths.Add(collider2D.GetPath(pathIndex).Select(p => offset + p).ToList());
            }
            return paths;
        }
    }
}
