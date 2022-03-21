using System;
using MarblePhysics.thirdparty;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [RequireComponent(typeof(PolygonCollider2D))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(LineRenderer))]
    public class ColliderToMesh : MonoBehaviour
    {
        // Based onh ttp://answers.unity3d.com/questions/835675/how-to-fill-polygon-collider-with-a-solid-color.html

        public enum Interior
        {
            None,
            Filled
        }

        public enum Outline
        {
            None,
            Open,
            Closed
        }

        public Interior interior = Interior.Filled;
        public Outline outline = Outline.Closed;

        public PolygonCollider2D polygonCollider2D;
        public MeshFilter meshFilter;

        public Mesh Mesh {
            get => meshFilter.sharedMesh;
            set => meshFilter.sharedMesh = value;
        }
        
        public MeshRenderer meshRenderer;
        public bool isWorldSpaceUV;

        public bool update = false;

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (update)
            {
                Init();
            }
        }

        public void Init()
        {
            CreateMesh();
        }

        void CreateMesh()
        {
            if (polygonCollider2D == null) polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
            if ((meshFilter == null || meshRenderer == null) && interior == Interior.None) return;
            if (meshFilter == null)
            {
                Debug.LogError(this + " has null meshFilter");
                return;
            }

            if (meshRenderer == null)
            {
                Debug.LogError(this + " has null meshRenderer");
                return;
            }

            if (interior == Interior.None)
            {
                meshRenderer.enabled = false;
                return;
            }
            else
            {
                meshRenderer.enabled = true;
            }


            int pointCount = 0;
            pointCount = polygonCollider2D.GetTotalPointCount();
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            else
            {
                mesh.Clear();    
            }
            
            Vector2[] points = polygonCollider2D.points;
            Vector3[] vertices = new Vector3[pointCount];
            Vector2[] uv = new Vector2[pointCount];
            for (int j = 0; j < pointCount; j++)
            {
                Vector2 actual = points[j];
                vertices[j] = new Vector3(actual.x, actual.y, 0);
                if (isWorldSpaceUV)
                {
                    uv[j] = actual;
                }
                else
                {
                    uv[j] = new Vector2(actual.x / polygonCollider2D.bounds.size.x, actual.y / polygonCollider2D.bounds.size.y);
                }
            }

            Triangulator tr = new Triangulator(points);
            int[] triangles = tr.Triangulate();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            meshFilter.sharedMesh = mesh;
        }
    }
}