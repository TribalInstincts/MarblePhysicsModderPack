using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public class SeatSorter : Sorter
    {
        [SerializeField]
        private SeatSet[] SeatSets = default;

        public override void Sort(Marble marble)
        {
            foreach (SeatSet seatSet in SeatSets)
            {
                if (seatSet.TryTakeMarble(marble))
                {
                    break;
                }
            }
        }
    }
}
