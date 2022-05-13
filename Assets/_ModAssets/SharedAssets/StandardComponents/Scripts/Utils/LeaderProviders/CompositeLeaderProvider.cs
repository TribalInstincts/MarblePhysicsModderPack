using System.Collections.Generic;
using UnityEngine;

namespace MarblePhysics.Modding
{
    /// <summary>
    /// Allows you to link multiple leader providers together for one ultimate list.
    /// </summary>
    public class CompositeLeaderProvider : LeaderProvider
    {
        [SerializeField]
        [Tooltip("IMPORTANT: Must be ordered First to Last in terms of the Leader. i.e.: The final zone of a race would be the FIRST entry.")]
        private LeaderProvider[] leaderProviders = default;

        public override IEnumerable<Leader> GetLeaders(bool forceRecalculation = false)
        {
            foreach (LeaderProvider leaderProvider in leaderProviders)
            {
                foreach (Leader leader in leaderProvider.GetLeaders(forceRecalculation))
                {
                    yield return leader;
                }
            }
        }

        protected override void LateUpdate()
        {
            // NoOp this just to avoid the base setting state needlessly.
        }

        protected override IEnumerable<Leader> CalculateLeaders()
        {
            // Nothing needs to happen here as each of the leader providers will manage their own state.
            return default;
        }
    }
}
