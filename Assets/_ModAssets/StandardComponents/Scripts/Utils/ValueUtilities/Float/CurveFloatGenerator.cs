using System;
using System.Linq;
using UnityEngine;

namespace MarblePhysics.Modding
{
    /// <summary>
    /// Returns a value from the time evaluation of an animation curve
    /// </summary>
    [Serializable]
    public class CurveFloatGenerator : FloatGenerator
    {
        [SerializeField]
        private AnimationCurve curve = default;

        [SerializeField, Tooltip("How the curve is evaluated when the time exceeds the keyframe values.")]
        private WrapMode wrapMode = WrapMode.Loop;

        protected override void Awake()
        {
            base.Awake();
            curve.preWrapMode = wrapMode;
            curve.postWrapMode = wrapMode;
        }

        public override float GetFixedValue()
        {
            return curve.Evaluate(GetFixedTime());
        }

        public override float GetMinimum()
        {
            return curve.keys.Select(k => k.value).Min();
        }

        public override float GetMaximum()
        {
            return curve.keys.Select(k => k.value).Max();
        }
    }
}