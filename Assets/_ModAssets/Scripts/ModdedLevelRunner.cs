using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarblePhysics;
using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Level;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace TribalInstincts
{
    public class ModdedLevelRunner : LevelRunner
    {
        private bool isGameOver = false;

        [SerializeField]
        private SpawnZone spawnZone = default;
        
        public override void Initialize(HashSet<PlayerReference> players)
        {
            base.Initialize(players);
        }

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
            return (Vector2.zero, 22, null);
        }

        public override IEnumerator RunGame()
        {
            foreach (Marble activePlayerEntry in ActivePlayerEntries)
            {
                activePlayerEntry.SetGameState(PlayerGameState.InPlay);
            }
            yield return new WaitForSeconds(15f);
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
