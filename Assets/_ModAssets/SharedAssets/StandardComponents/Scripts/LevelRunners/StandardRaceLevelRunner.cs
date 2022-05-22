using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Level;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public class StandardRaceLevelRunner : LevelRunner
    {
        [SerializeField]
        private CameraController cameraController = default;

        [SerializeField]
        private SpawnZone spawnZone = default;

        [SerializeField] 
        private LeaderProvider leaderProvider = default;
        
        private bool isGameOver = false;
        
        public override IEnumerator PrepareGame()
        {
            spawnZone.PlaceMarbles(InitialPlayers.Select(p =>
            {
                Marble marble = SpawnMarble(p);
                marble.gameObject.SetActive(true);
                return marble;
            }).ToArray());

            yield break;
        }

        public override (Vector2 position, float size, Bounds2D?) GetCameraPlacement(Camera forCamera)
        {
            return cameraController.GetCameraPlacement(forCamera);
        }

        public override IEnumerator RunGame()
        {
            foreach (Marble activePlayerEntry in ActivePlayerEntries)
            {
                activePlayerEntry.SetGameState(PlayerGameState.InPlay);
            }

            yield return new WaitUntil(() => isGameOver);
            print("Game over");
        }

        public override IEnumerable<PlayerResult> GetPlayerResults()
        {
            int placement = 0;
            return leaderProvider.GetLeaders(true, true).Select(leader => new PlayerResult { Player = leader.PlayerReference, Placement = placement++});
        }

        public void EndGame()
        {
            isGameOver = true;
        }
    }
}
