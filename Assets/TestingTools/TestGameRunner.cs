using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Level;
using MarblePhysics.Modding.Shared.Player;
using MarblePhysics.Modding.Shared.Theme;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TribalInstincts
{
    /// <summary>
    /// DO NOT USE THIS in your level creation, or anything under the TestTools namespace. They will change without warning.
    /// </summary>
    public class TestGameRunner : MarbleSpawner
    {
        [SerializeField]
        private Marble marblePrefab = default;

        [SerializeField]
        private CameraManager cameraManager = default;

        private LevelRunner levelRunner;

        private void Start()
        {
            if (TryGetValidLevelRunner(out LevelRunner levelRunner))
            {
                this.levelRunner = levelRunner;
                StartCoroutine(RunGame(levelRunner));
            }
            else
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }
        }

        private bool TryGetValidLevelRunner(out LevelRunner levelRunner)
        {
            bool hasErrors = false;

            levelRunner = null;

            LevelRunner[] levelRunnersInScene = FindObjectsOfType<LevelRunner>();
            if (levelRunnersInScene.Length != 1)
            {
                Debug.LogError("You must have exactly one LevelRunner instance in the scene.");
                return false;
            }

            levelRunner = levelRunnersInScene[0];

            if (levelRunner.LayerConfig == null)
            {
                Debug.LogError("Please add a CollisionMatrix to your Level and bind it to your LevelRunner!");
                hasErrors = true;
            }

            return !hasErrors;
        }

        private IEnumerator RunGame(LevelRunner levelRunner)
        {
            levelRunner.LayerConfig.CollisionMatrix.Apply();
            ThemeManager.Instance.ChangeToRandomTheme();
            Debug.Log("Starting game");
            cameraManager.SetCameraController(levelRunner);
            LevelConfig config = levelRunner.LevelConfig;
            HashSet<PlayerReference> botPlayers = CreateBotPlayers(Random.Range(config.MinPlayers, config.MaxPlayers));
            levelRunner.Initialize(botPlayers);
            yield return levelRunner.PrepareGame();
            yield return levelRunner.RunGame();
            IEnumerable<PlayerResult> playerResults = levelRunner.GetPlayerResults();
            Debug.Log("Game over");
        }

        private HashSet<PlayerReference> CreateBotPlayers(int count)
        {
            return new(Enumerable.Range(0, count).Select(i => new PlayerReference("Bot#" + Random.Range(1, 100000), true)));
        }

        /// <summary>
        /// Do not call this class directly(or anything under the .Mod.TestTools namespace).
        /// To spawn a marble, call the base MarbleSpawner.Instance.AcquireInstance
        /// </summary>
        public override Marble AcquireInstance(PlayerReference playerReference)
        {
            Marble newMarble = Instantiate(marblePrefab);
            newMarble.Init(levelRunner.LayerConfig.DefaultMarbleLayer);
            newMarble.SetPlayer(playerReference);
            return newMarble;
        }

        /// <summary>
        /// Do not call this class directly(or anything under the .Mod.TestTools namespace).
        /// To release a marble, call the base MarbleSpawner.Instance.ReleaseInstance
        /// </summary>
        public override void ReleaseInstance(Marble marble)
        {
            marble.gameObject.SetActive(false);
            marble.Reset();
            Destroy(marble.gameObject);
        }

        /// <summary>
        /// Do not call this class directly(or anything under the .Mod.TestTools namespace).
        /// To release all marbles, call the base MarbleSpawner.Instance.ReleaseAll
        /// </summary>
        public override void ReleaseAll()
        {
            foreach (Marble marble in FindObjectsOfType<Marble>())
            {
                ReleaseInstance(marble);
            }
        }
    }
}