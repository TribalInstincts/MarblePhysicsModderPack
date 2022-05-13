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

        private List<Leader> currentLeaders;

        protected virtual void LateUpdate()
        {
            RecalculateLeaders();
        }

        /// <summary>
        /// Returns an ordered enumerable of playerReferences and an associated active marble, if it has one. This list is not guaranteed to consist of distinct player references.
        /// </summary>
        public virtual IEnumerable<Leader> GetLeaders(bool forceRecalculation = false)
        {
            if (currentLeaders == null || forceRecalculation)
            {
                RecalculateLeaders();
            }

            return currentLeaders;
        }

        private void RecalculateLeaders()
        {
            currentLeaders ??= new List<Leader>();
            currentLeaders.Clear();

            currentLeaders.AddRange(CalculateLeaders());
        }

        protected abstract IEnumerable<Leader> CalculateLeaders();
    }
}