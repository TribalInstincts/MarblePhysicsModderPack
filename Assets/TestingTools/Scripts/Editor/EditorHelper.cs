using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarblePhysics.Modding.Test
{
    [InitializeOnLoad]
    public static class EditorHelper
    {
        private static Scene testScene;
        
        static EditorHelper()
        {
            EditorApplication.projectChanged += EditorApplicationOnprojectChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
        }

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                TestGameRunner gameRunner = GameObject.FindObjectOfType<TestGameRunner>();
                if (gameRunner == null)
                {
                    EditorSceneManager.OpenScene("Assets/TestingTools/Resources/GameRunner.unity", OpenSceneMode.Additive);
                }
            } 
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                TestGameRunner gameRunner = GameObject.FindObjectOfType<TestGameRunner>();
                if (gameRunner != null)
                {
                    EditorSceneManager.CloseScene(gameRunner.gameObject.scene, true);
                }
            }
        }

        private static void EditorApplicationOnprojectChanged()
        {
            string[] guids = AssetDatabase.FindAssets("t:scene", new[] {"Assets/_ModAssets"});
            if (guids.Length != EditorBuildSettings.scenes.Length)
            {
                string[] paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
                EditorBuildSettings.scenes = paths.Select(path => new EditorBuildSettingsScene(path, true)).ToArray();
            }
        }
    }
}
