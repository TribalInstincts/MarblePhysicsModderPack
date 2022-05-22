using System.Collections.Generic;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public abstract class LeaderProvider : MonoBehaviour
    {
        public readonly struct Leader
        {
            public readonly PlayerReference PlayerReference;
            private readonly bool hasActiveMarble;
            private readonly Marble marble;

            public Leader(Marble marble) : this(marble.PlayerReference, marble){}
            public Leader(PlayerReference playerReference, Marble marble = null)
            {
                PlayerReference = playerReference;
                this.marble = marble;
                hasActiveMarble = marble != null;
            }

            public bool TryGetActiveMarble(out Marble marble)
            {
                marble = this.marble;
                return hasActiveMarble;
            }
        }

        private List<Leader> allLatestLeaders;
        private List<Leader> uniqueLatestLeaders;
        private HashSet<PlayerReference> currentPlayerReferences;

        protected virtual void LateUpdate()
        {
            RecalculateLeaders();
        }

        /// <summary>
        /// Returns an ordered enumerable of playerReferences and an associated active marble, if it has one.
        /// If singleEntryPerPlayer is enabled, only the best placement is returned per player entry
        /// </summary>
        public virtual IEnumerable<Leader> GetLeaders(bool singleEntryPerPlayer = false, bool forceRecalculation = false)
        {
            if (allLatestLeaders == null || forceRecalculation)
            {
                RecalculateLeaders();
            }

            return singleEntryPerPlayer ? uniqueLatestLeaders : allLatestLeaders;
        }

        private void RecalculateLeaders()
        {
            CacheLeaders(CalculateLeaders());
        }

        protected void CacheLeaders(IEnumerable<Leader> leaders)
        {
            allLatestLeaders ??= new List<Leader>();
            uniqueLatestLeaders ??= new List<Leader>();
            currentPlayerReferences ??= new HashSet<PlayerReference>();
            
            currentPlayerReferences.Clear();
            allLatestLeaders.Clear();
            uniqueLatestLeaders.Clear();

            foreach (Leader leader in leaders)
            {
                allLatestLeaders.Add(leader);
                if (currentPlayerReferences.Add(leader.PlayerReference))
                {
                    uniqueLatestLeaders.Add(leader);
                }
            }
        }

        protected abstract IEnumerable<Leader> CalculateLeaders();
    }
}