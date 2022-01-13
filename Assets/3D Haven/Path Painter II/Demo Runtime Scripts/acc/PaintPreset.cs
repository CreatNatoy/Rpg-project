using UnityEngine;
using System;

namespace Haven.Demo
{
    /// <summary>
    /// Simple preset for path painting
    /// </summary>
    [Serializable]
    public struct PaintPreset
    {
        public float PathSize;
        public float EmbankmentSize;
        public float SlopeLimit;
        public float Ramp;
        public TerrainLayer PathLayer;
        public TerrainLayer EmbankmentLayer;

        public PaintPreset(float pathSize, float embankmentSize, float slopeLimit, float ramp,
            TerrainLayer pathLayer, TerrainLayer embankmentLayer)
        {
            PathSize = pathSize;
            EmbankmentSize = embankmentSize;
            SlopeLimit = slopeLimit;
            Ramp = ramp;
            PathLayer = pathLayer;
            EmbankmentLayer = embankmentLayer;
        }
    }
}
