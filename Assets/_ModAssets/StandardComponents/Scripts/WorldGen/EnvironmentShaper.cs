using System.Linq;
using ClipperLib;
using UnityEngine;
using UnityEngine.Serialization;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;


namespace MarblePhysics.Modding
{
    [ExecuteAlways]
    [RequireComponent(typeof(PolygonCollider2D))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public class EnvironmentShaper : MonoBehaviour
    {
        private static readonly float intScale = 10000f;
        
        [FormerlySerializedAs("source")]
        [SerializeField]
        private PolygonCollider2D canvasCollider = default;
        [SerializeField]
        private PolygonCollider2D[] cutColliders = default;
        public PolygonCollider2D[] CutColliders => cutColliders;

        [SerializeField]
        private bool cut = false;

        private bool isInitialized = false;
        private PolygonCollider2D resultCollider = default;
        private MeshFilter meshFilter = default;

        private void Init()
        {
            if (!isInitialized)
            {
                resultCollider = GetComponent<PolygonCollider2D>();
                meshFilter = GetComponent<MeshFilter>();
                
                isInitialized = true;
            }
        }

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            Init();
            if (cut)
            {
                CutShape();
                UpdateMesh();
            }
        }

        private void UpdateMesh()
        {
            Mesh mesh = resultCollider.CreateMesh(true, true);
            Vector3[] meshVertices = mesh.vertices;
            for (int i = 0; i < meshVertices.Length; i++)
            {
                meshVertices[i] = resultCollider.transform.InverseTransformPoint(meshVertices[i]);
            }
            mesh.SetVertices(meshVertices);
            Mesh currentMesh = meshFilter.sharedMesh;
            if (currentMesh != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(currentMesh);
                }
                else
                {
                    DestroyImmediate(currentMesh);
                }
            }
            meshFilter.sharedMesh = mesh;
        }

        private void CutShape()
        {
            Paths solution = new Paths();
            Clipper clipper = new Clipper();
            Paths sourcePath = GetPaths(canvasCollider);
            clipper.AddPaths(sourcePath, PolyType.ptSubject, true);
            
            foreach (PolygonCollider2D polygonCollider2D in cutColliders)
            {
                Paths clip = GetPaths(polygonCollider2D);
                clipper.AddPaths(clip, PolyType.ptClip, true);
            }
            
            clipper.Execute(ClipType.ctDifference, solution, PolyFillType.pftNonZero);

            UpdateResultCollider(resultCollider, solution);
        }

        private static Paths GetPaths(PolygonCollider2D collider2D)
        {
            Paths paths = new Paths(collider2D.pathCount);
            for (int pathIndex = 0; pathIndex < collider2D.pathCount; pathIndex++)
            {
                Vector2[] pathPoints = collider2D.GetPath(pathIndex);
                Path path = new Path(pathPoints.Length);
                path.AddRange(pathPoints.Select(p => Convert(collider2D.transform.TransformPoint(p))));
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
                collider.SetPath(pathIndex, points.Select(p => (Vector2)collider.transform.InverseTransformPoint(Convert(p))).ToArray());
            }
        }

        private static IntPoint Convert(Vector2 vector2) => new(vector2.x * intScale, vector2.y * intScale);
        private static Vector2 Convert(IntPoint intPoint) => new(intPoint.X / intScale, intPoint.Y / intScale);
    }
}
