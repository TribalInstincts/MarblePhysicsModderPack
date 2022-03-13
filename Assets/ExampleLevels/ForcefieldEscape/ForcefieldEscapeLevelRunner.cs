using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Level;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics
{
    public class ForcefieldEscapeLevelRunner : LevelRunner
    {
        private bool isGameOver = false;
        
        public override void Initialize(HashSet<PlayerReference> players)
        {
            base.Initialize(players);
        }

        public override IEnumerator PrepareGame()
        {
            foreach (PlayerReference playerReference in InitialPlayers)
            {
                Marble marble = SpawnMarble(playerReference);
                marble.transform.position = Random.insideUnitCircle * 2;
                marble.gameObject.SetActive(true);
            }

            yield break;
        }

        public override (Vector2 position, float size, Bounds2D?) GetCameraPlacement(Camera forCamera)
        {
            return (Vector2.zero, 22, null);
        }

        public override IEnumerator RunGame()
        {
            foreach (Marble activePlayerEntry in ActivePlayerEntries)
            {
                activePlayerEntry.SetGameState(PlayerGameState.InPlay);
            }
            yield return new WaitForSeconds(5f);
            isGameOver = true;
        }

        public override IEnumerable<PlayerResult> GetPlayerResults()
        {
            return InitialPlayers.Select(player => new PlayerResult {Player = player});
        }

        public void EndGame()
        {
            isGameOver = true;
        }
    }
}
