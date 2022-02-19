using System;
using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public class Rotator : MonoBehaviour
    {
        [Serializable]
        public enum OscillationMethod
        {
            Linear,
            Sin,
            PingPong,
            Curve
        }

        [SerializeField]
        private float startingAngle = default;
        [SerializeField]
        private float endingAngle = default;
    }

}