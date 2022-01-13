using UnityEngine;
using System;

namespace Haven.Demo
{
    /// <summary>
    /// Simple float value with a label assigned
    /// </summary>
    [Serializable]
    public struct LabeledFloat
    {
        public string Label;
        public float Value;

        public LabeledFloat(string label, float value)
        {
            Label = label;
            Value = value;
        }
    }
}
