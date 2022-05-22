using System;
using System.Linq;
using MarblePhysics.Modding.Shared.Level;
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
        
        public int FilledCount { get; private set; }
        public int SeatCount => seats.Length;
        public float FillRatio => (float)FilledCount / SeatCount;
        public bool IsFull => FilledCount == SeatCount;
        public float LastTakeTime { get; private set; }

        public Marble[] Marbles => seats.Select(s => s.Marble).Where(m => m != null).ToArray();

        public virtual bool TryTakeMarble(Marble marble)
        {
            if (!IsFull && (filter == null || filter.PassesFilter(marble)))
            {
                foreach (Seat seat in seats)
                {
                    if (seat.TryTakeMarble(marble))
                    {
                        FilledCount++;
                        LastTakeTime = LevelRunner.Instance.CurrentRunTime;
                        if (IsFull)
                        {
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