using System.Collections.Generic;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    /// <summary>
    /// Provides current leaders based on 1 or more SeatSets. If more than one seatset is provided, the returned leaders will be which ever marbles are in the most filled set.
    /// After the current leading seatset is decided(or if there is only one), the leaders are returned in the order they finished.
    /// </summary>
    public class FinishLineLeaderProvider : LeaderProvider
    {
        [SerializeField]
        private SeatSet[] seatSets = default;
        
        protected override IEnumerable<Leader> CalculateLeaders()
        {
            SeatSet leadingSet = null;

            switch (seatSets.Length)
            {
                case 1:
                    leadingSet = seatSets[0];
                    break;
                case > 1:
                {
                    bool foundFilled = false;
                    float lastTakenTime = Mathf.Infinity;
                    float mostFilledRatio = 0;
                    
                    foreach (SeatSet seatSet in seatSets)
                    {
                        if (foundFilled)
                        {
                            if (seatSet.IsFull && seatSet.LastTakeTime <= lastTakenTime)
                            {
                                leadingSet = seatSet;
                                lastTakenTime = seatSet.LastTakeTime;
                            }
                        }
                        else if (seatSet.FillRatio > mostFilledRatio)
                        {
                            if (seatSet.IsFull)
                            {
                                foundFilled = true;
                            }

                            leadingSet = seatSet;
                            mostFilledRatio = seatSet.FillRatio;
                            lastTakenTime = seatSet.LastTakeTime;
                        }
                        else if (Mathf.Approximately(seatSet.FillRatio, mostFilledRatio) && seatSet.LastTakeTime < lastTakenTime)
                        {
                            leadingSet = seatSet;
                            lastTakenTime = seatSet.LastTakeTime;
                        }
                    }

                    break;
                }
            }

            if (leadingSet != null)
            {
                foreach (Marble marble in leadingSet.Marbles)
                {
                    yield return new Leader(marble);
                }
            }
        }
    }
}
