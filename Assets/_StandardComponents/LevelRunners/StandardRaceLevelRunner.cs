using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Level;
using MarblePhysics.Modding.Shared.Player;
using MarblePhysics.Modding.StandardComponents;
using UnityEngine;

namespace MarblePhysics
{
    public class StandardRaceLevelRunner : LevelRunner
    {
        [SerializeField]
        private CameraController cameraController = default;

        [SerializeField]
        private SpawnZone spawnZone = default;
        
        private bool isGameOver = false;

        private Marble[] winners;
        
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
            return winners.Select(player => new PlayerResult {Player = player.PlayerReference, Placement = placement++});
        }

        public void EndGame(SeatSet winningSeatSet)
        {
            this.winners = winningSeatSet.Marbles;
            isGameOver = true;
        }
    }
}
