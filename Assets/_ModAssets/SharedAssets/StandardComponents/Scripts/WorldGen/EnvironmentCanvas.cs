using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using UnityEngine;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;


namespace MarblePhysics.Modding
{
    [RequireComponent(typeof(PolygonCollider2D))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class EnvironmentCanvas : MonoBehaviour
    {
        private const float ClipperConversionScale = 1000f;

        private PolygonCollider2D resultCollider = default;
        private MeshFilter meshFilter = default;

        private Paths sourcePath = default;
        private Paths solution = null;
        private Clipper clipper = null;
        private Collider2D[] overlappingColliders;
        public Bounds Bounds => resultCollider.bounds;

        [SerializeField]
        private bool cutOnce = false;

        public void Init()
        {
            if (clipper == null)
            {
                overlappingColliders = new Collider2D[0];
                clipper = new Clipper();
                solution = new Paths();
                resultCollider = GetComponent<PolygonCollider2D>();
                meshFilter = GetComponent<MeshFilter>();
                meshFilter.sharedMesh = new Mesh();
            }
        }

        private void Awake()
        {
            Init();
        }


        private void Start()
        {
            CutHoles();
        }

        private void Update()
        {
            if (cutOnce)
            {
                CutHoles();
                cutOnce = false;
            }
        }

        public void CutHoles()
        {
            CutShape();
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            Mesh mesh = resultCollider.CreateMesh(true, true);
            Vector3[] meshVertices = mesh.vertices;
            for (int i = 0; i < meshVertices.Length; i++)
            {
                meshVertices[i] = resultCollider.transform.InverseTransformPoint(meshVertices[i]);
            }

            meshFilter.sharedMesh.Clear();
            meshFilter.sharedMesh.vertices = meshVertices;
            meshFilter.sharedMesh.triangles = mesh.triangles;
        }

        private void CutShape()
        {
            UpdateSourcePath();
            clipper.Clear();
            clipper.AddPaths(sourcePath, PolyType.ptSubject, true);
            foreach (Collider2D collider2D in Physics2D.OverlapBoxAll(resultCollider.bounds.center, resultCollider.bounds.size, transform.localEulerAngles.z))
            {
                if (collider2D.gameObject.TryGetComponent(out CutVolume cutVolume))
                {
                    if (cutVolume.TryGetPaths(out Paths paths))
                    {
                        clipper.AddPaths(paths, PolyType.ptClip, true);
                    }
                }
            }

            solution.Clear();
            clipper.Execute(ClipType.ctDifference, solution, PolyFillType.pftNonZero);
            UpdateResultCollider(resultCollider, solution);
        }

        private void UpdateSourcePath()
        {
            Vector2 halfSize = Vector3.one * .5f;
            this.sourcePath = new Paths
            {
                new()
                {
                    LocalPosToWorldInt(transform, new Vector2(-halfSize.x, -halfSize.y)), // down left
                    LocalPosToWorldInt(transform, new Vector2(halfSize.x, -halfSize.y)), // down right
                    LocalPosToWorldInt(transform, new Vector2(halfSize.x, halfSize.y)), // up right
                    LocalPosToWorldInt(transform, new Vector2(-halfSize.x, halfSize.y)), // up left
                }
            };
        }

        private static Paths GetPaths(PolygonCollider2D collider2D)
        {
            Paths paths = new Paths(collider2D.pathCount);
            for (int pathIndex = 0; pathIndex < collider2D.pathCount; pathIndex++)
            {
                Vector2[] pathPoints = collider2D.GetPath(pathIndex);
                Path path = new Path(pathPoints.Length);
                path.AddRange(pathPoints.Select(p => LocalPosToWorldInt(collider2D.transform, p)));
                paths.Add(path);
            }

            return paths;
        }

        private static void UpdateResultCollider(PolygonCollider2D collider, Paths paths)
        {
            collider.pathCount = paths.Count;
            for (int pathIndex = 0; pathIndex < paths.Count; pathIndex++)
            {
                Path points = paths[pathIndex];
                collider.SetPath(pathIndex, points.Select(p => WorldIntToLocalPos(collider.transform, p)).ToArray());
            }
        }

        // TO OPTIMIZE: GC
        public static Paths LocalPathsToWorldIntPaths(Transform transform, List<List<Vector2>> localPaths)
        {
            Paths paths = new Paths(localPaths.Count);
            foreach (List<Vector2> localPath in localPaths)
            {
                Path path = new Path(localPath.Count);
                foreach (Vector2 vector2 in localPath)
                {
                    path.Add(LocalPosToWorldInt(transform, vector2));
                }

                paths.Add(path);
            }

            return paths;
        }

        public static IntPoint LocalPosToWorldInt(Transform transform, Vector2 position) => WorldPosToWorldInt(transform.TransformPoint(position));
        public static IntPoint WorldPosToWorldInt(Vector2 vector2) => new(vector2.x * ClipperConversionScale, vector2.y * ClipperConversionScale);
        public static Vector2 WorldIntToLocalPos(Transform transform, IntPoint intPoint) => transform.InverseTransformPoint(WorldIntToWorldPos(intPoint));
        public static Vector2 WorldIntToWorldPos(IntPoint intPoint) => new(intPoint.X / ClipperConversionScale, intPoint.Y / ClipperConversionScale);
    }
}