using System;
using System.Collections.Generic;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents.Filters
{
    /// <summary>
    /// Checks if the set time has passed since the last CheckFilter call was made.
    /// </summary>
    public class CooldownMarbleFilter : MarbleFilter
    {
        [Serializable]
        public enum Scope
        {
            Global,
            PerMarble
        }

        [SerializeField]
        private Scope scope = default;

        [SerializeField, Tooltip("Time in seconds")]
        private float cooldownTime = 5f;
        
        private Dictionary<Marble, float> marbleCooldownEndTimes;
        private float globalCooldownEndtime = -1f;
        
        private void Awake()
        {
            if (scope == Scope.PerMarble)
            {
                marbleCooldownEndTimes = new Dictionary<Marble, float>();
            }
        }

        public override bool PassesFilter(Marble marble)
        {
            float time = Time.time;

            if (scope == Scope.PerMarble)
            {
                if (marbleCooldownEndTimes.TryGetValue(marble, out float cooldownEndTime) && time < cooldownEndTime)
                {
                    return false;
                }
                else
                {
                    marbleCooldownEndTimes[marble] = time + cooldownTime;
                    return true;
                }
            }
            else
            {
                if (time < globalCooldownEndtime)
                {
                    return false;
                }
                else
                {
                    globalCooldownEndtime = time + cooldownTime;
                    return true;
                }
            }
        }
    }
}
