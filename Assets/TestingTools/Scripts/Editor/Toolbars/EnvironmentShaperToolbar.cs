using System.Collections;
using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Extensions;
using MarblePhysics.Modding.Shared.Theme;
using MarblePhysics.Modding.Test;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace MarblePhysics.Modding
{
    [Overlay(typeof(SceneView), "MarblePhysics/Environment Shaper Toolbar")]
    [Icon(EditorIcon.Paths.icon_align_bottom)]
    public class EnvironmentShaperToolbar : Overlay
    {
        private int pointCount = 4;
        private bool isCutVolume = false;

        private bool includeVisual;
        private bool visual;
        private ThemeElementCategory elementCategory = ThemeElementCategory.Environment;
        private Color color = Color.white.WithAlpha(.5f);

        private Plane plane;
        private int envLayer;

        public override VisualElement CreatePanelContent()
        {
            plane = new Plane(Vector3.forward, 0);
            if (LayerConfig.TryGetInstance(out LayerConfig layerConfig))
            {
                envLayer = layerConfig.DefaultEnvironmentLayer;
            }

            return new IMGUIContainer(DrawGUI);
        }

        private void DrawGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (isCutVolume)
                {
                    isCutVolume = !GUILayout.Toggle(false, new GUIContent("Standard", ""), "Button");
                    GUILayout.Toggle(true, new GUIContent("EnvironmentCanvas Hole", ""), "Button");
                }
                else
                {
                    GUILayout.Toggle(true, new GUIContent("Standard", ""), "Button");
                    isCutVolume = GUILayout.Toggle(false, new GUIContent("EnvironmentCanvas Hole", ""), "Button");
                }
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            {
                includeVisual = EditorGUILayout.Toggle("Include Visual:", includeVisual);

                GUI.enabled = includeVisual;

                if (isCutVolume)
                {
                    if (includeVisual)
                    {
                        color = EditorGUILayout.ColorField(color);
                    }
                    else
                    {
                        EditorGUILayout.ColorField(color);
                    }
                }
                else
                {
                    if (includeVisual)
                    {
                        elementCategory = (ThemeElementCategory) EditorGUILayout.EnumPopup(elementCategory);
                    }
                    else
                    {
                        EditorGUILayout.EnumPopup(elementCategory);
                    }
                }

                GUI.enabled = true;
            }  EditorGUILayout.EndHorizontal();

            
            pointCount = EditorGUILayout.IntSlider("Sides:", pointCount, 3, 10);
            
            if (isCutVolume)
            {
                GUI.enabled = false;
                EditorGUILayout.LayerField("Layer: ", LayerMask.NameToLayer("TransparentFX"));
                GUI.enabled = true;
            }
            else
            {
                envLayer = EditorGUILayout.LayerField("Layer: ", envLayer);
            }


            string shapeName = pointCount switch
            {
                3 => "Triangle",
                4 => "Square",
                5 => "Pentagon",
                6 => "Hexagon",
                7 => "Heptagon",
                8 => "Octagon",
                9 => "Nonagon",
                _ => $"Circle"
            };
            if (GUILayout.Button("Spawn " + shapeName))
            {
                SpawnCutVolume(pointCount, shapeName);
            }
        }


        protected GameObject SpawnCutVolume(int sideCount, string shapeName)
        {
            GameObject instance = new GameObject((isCutVolume ? "EnvCanvasHole-" : "Shape-") + shapeName);
            if (Selection.activeGameObject != null)
            {
                instance.transform.SetParent(Selection.activeGameObject.transform);
            }

            if (isCutVolume)
            {
                instance.layer = LayerMask.NameToLayer("TransparentFX");
            }
            else
            {
                instance.layer = envLayer;
            }

            Collider2D collider;
            if (sideCount == 4)
            {
                collider = instance.AddComponent<BoxCollider2D>();
            }
            else if (sideCount >= 10)
            {
                collider = instance.AddComponent<CircleCollider2D>();
            }
            else
            {
                collider = instance.AddComponent<PolygonCollider2D>();
            }

            CutVolume cutVolume = null;
            if (isCutVolume)
            {
                cutVolume = instance.AddComponent<CutVolume>();
            }

            if (includeVisual)
            {
                switch (collider)
                {
                    case BoxCollider2D:
                        AddSprite(instance, "box");
                        break;
                    case CircleCollider2D:
                        AddSprite(instance, "circle");
                        break;
                    case PolygonCollider2D:
                        AddSpriteShape(instance, sideCount, cutVolume);
                        break;
                }

                if (!isCutVolume)
                {
                    instance.AddComponent<ThemeColorSetter>().Configure(elementCategory);
                }
            }

            Undo.RegisterCreatedObjectUndo(instance, "Create EnvShaper element");

            bool foundPosition = false;
            if (containerWindow is SceneView sceneView)
            {
                Ray ray = sceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
                if (plane.Raycast(ray, out float dist))
                {
                    foundPosition = true;
                    instance.transform.position = ray.GetPoint(dist);
                }
            }

            if (!foundPosition)
            {
                instance.transform.ClearLocal();
            }

            return instance;
        }

        private void AddSprite(GameObject instance, string spriteName)
        {
            SpriteRenderer sr = instance.AddComponent<SpriteRenderer>();
            if (isCutVolume)
            {
                sr.color = color;
            }
            sr.sprite = Resources.Load<Sprite>("EnvironmentShaper/Primitives/" + spriteName);
        }

        private void AddSpriteShape(GameObject instance, int sides, CutVolume cutVolume = null)
        {
            SpriteShapeRenderer ssr = instance.AddComponent<SpriteShapeRenderer>();
            if (isCutVolume)
            {
                ssr.color = color;
            }
            SpriteShapeController ssc = instance.AddComponent<SpriteShapeController>();
            ssc.spline.Clear();
            
            float radius = Mathf.Sqrt(2) * .5f;
            float radDelta = 360.0f / sides * Mathf.Deg2Rad;
            float offset = 90f * Mathf.Deg2Rad;

            ssc.spline.Clear();
            for (int i = 0; i < sides; i++)
            {
                float angle = (i * radDelta) + offset;
                ssc.spline.InsertPointAt(0, new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)));
            }

            ssc.spriteShape = Resources.Load<SpriteShape>("StandardSpriteShape");
            ssc.autoUpdateCollider = true;
            ssc.StartCoroutine(BakeNextFrame(ssc, cutVolume));
        }

        private IEnumerator BakeNextFrame(SpriteShapeController ssc, CutVolume cutVolume = null)
        {
            yield return null;
            ssc.BakeCollider();
            if (cutVolume != null)
            {
                yield return null;
                cutVolume.ForcePathUpdate();
            }
        }
    }
}