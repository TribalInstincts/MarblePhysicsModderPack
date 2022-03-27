using System;
using System.Linq;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;
using UnityEngine.Events;

namespace MarblePhysics.Modding
{
    [Serializable]
    public class SeatSetUnityEvent : UnityEvent<SeatSet> {
    }
    
    public class SeatSet : MonoBehaviour
    {
        public SeatSetUnityEvent SeatSetFilled = default;
        
        [SerializeField]
        private Seat[] seats = default;

        [SerializeField]
        private MarbleFilter filter = default;
        
        private bool isSetFull = false;

        public Marble[] Marbles => seats.Select(s => s.Marble).ToArray();

        public virtual bool TryTakeMarble(Marble marble)
        {
            if (!isSetFull && (filter == null || filter.PassesFilter(marble)))
            {
                for (int index = 0; index < seats.Length; index++)
                {
                    Seat seat = seats[index];
                    if (seat.TryTakeMarble(marble))
                    {
                        if (index == seats.Length - 1)
                        {
                            isSetFull = true;
                            SeatSetFilled?.Invoke(this);
                        }
                        return true;
                    }
                }
                
            }

            return false;
        }
    }
}