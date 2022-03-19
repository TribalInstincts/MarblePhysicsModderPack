using MarblePhysics.Modding.Shared;
using MarblePhysics.Modding.Shared.Player;
using MarblePhysics.Modding.StandardComponents;
using UnityEngine;

namespace TribalInstincts
{
    /// <summary>
    /// Same events as the bas MarbleCollisionHandler, just piped out to the inspector for wiring.
    /// </summary>
    public class UnityEventMarbleCollisionHandler : MarbleCollisionHandler
    {
        public MarbleUnityEvent OnMarbleEnter = default;
        public MarbleUnityEvent OnMarbleStay = default;
        public MarbleUnityEvent OnMarbleExit = default;
        
        protected override void OnMarbleTriggerEnter(Marble marble)
        {
            OnMarbleEnter?.Invoke(marble);
        }

        protected override void OnMarbleTriggerExit(Marble marble)
        {
            OnMarbleExit?.Invoke(marble);
        }

        protected override void OnMarbleTriggerStay(Marble marble)
        {
            OnMarbleStay?.Invoke(marble);
        }

        protected override void OnMarbleCollisionEnter(Marble marble, Collision2D other)
        {
            OnMarbleEnter?.Invoke(marble);
        }

        protected override void OnMarbleCollisionExit(Marble marble, Collision2D other)
        {
            OnMarbleExit?.Invoke(marble);
        }

        protected override void OnMarbleCollisionStay(Marble marble, Collision2D other)
        {
            OnMarbleStay?.Invoke(marble);
        }
    }
}
