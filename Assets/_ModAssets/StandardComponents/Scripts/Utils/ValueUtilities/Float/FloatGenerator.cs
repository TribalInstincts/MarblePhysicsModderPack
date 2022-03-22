using UnityEngine;

namespace MarblePhysics.Modding
{
    /// <summary>
    /// Base class for a series of classes to help generate commonly used values.
    /// </summary>
    public abstract class FloatGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("Offsets the time evaluation by a random offset.")]
        private bool randomStartValue = true;

        [SerializeField, Tooltip("The multiplier that is applied to time before evaluating the curve.")]
        private float timeSpeed = 1f;

        private float timeOffset = 0;

        protected virtual void Awake()
        {
            if (randomStartValue)
            {
                timeOffset = Random.Range(0f, 1000f);
            }
        }

        protected float GetFixedTime()
        {
            float time = Time.fixedTime;
            if (!Application.isPlaying)
            {
                if (randomStartValue && timeOffset == 0)
                {
                    timeOffset = Random.Range(0f, 1000f);
                }
                time = Time.realtimeSinceStartup;
            }
            return (time + timeOffset) * timeSpeed;
        }

        public abstract float GetFixedValue();
        public abstract float GetMinimum();
        public abstract float GetMaximum();
    }
}