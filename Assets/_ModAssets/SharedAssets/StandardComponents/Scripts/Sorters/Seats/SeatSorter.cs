using MarblePhysics.Modding.Shared.Player;
using MarblePhysics.Modding;
using UnityEngine;

namespace MarblePhysics.Modding
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
