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

        protected override IEnumerable<Leader> CalculateLeaders()
        {
            foreach (LeaderProvider leaderProvider in leaderProviders)
            {
                foreach (Leader leader in leaderProvider.GetLeaders())
                {
                    yield return leader;
                }
            }
        }
    }
}
