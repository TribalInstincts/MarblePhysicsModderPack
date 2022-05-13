using System;
using System.Collections.Generic;
using System.Linq;
using MarblePhysics.Modding.Shared.Level;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    [RequireComponent(typeof(Zone))]
    public class ZoneLeaderProvider : LeaderProvider
    {
        [Serializable]
        private enum LeaderMethod
        {
            Undefined,
            Distance_Closest,
            Distance_Farthest,
            EnterTime
        }

        [SerializeField]
        private LeaderMethod leaderMethod = default;

        [SerializeField]
        private Transform relativeToPoint = default;

        private bool isInitialized;
        private Zone zone = default;

        private void Init()
        {
            if (!isInitialized)
            {
                zone = GetComponent<Zone>();
                isInitialized = true;
            }
        }

        protected override IEnumerable<Leader> CalculateLeaders()
        {
            Init();
            if (zone.HasPlayers)
            {
                return (leaderMethod switch
                {
                    LeaderMethod.Distance_Closest => zone.CurrentPlayers.Where(p => p.GameState != PlayerGameState.Disqualified)
                        .OrderBy(p => (p.transform.position - relativeToPoint.position).sqrMagnitude),
                    LeaderMethod.Distance_Farthest => zone.CurrentPlayers.Where(p => p.GameState != PlayerGameState.Disqualified)
                        .OrderByDescending(p => (p.transform.position - relativeToPoint.position).sqrMagnitude),
                    LeaderMethod.EnterTime => zone.CurrentPlayers.Where(p => p.GameState != PlayerGameState.Disqualified).OrderBy(zone.GetEnterTime),
                    _ => throw new ArgumentOutOfRangeException(leaderMethod.ToString())
                }).Select(p => new Leader(p));
            }

            return default;
        }
    }
}