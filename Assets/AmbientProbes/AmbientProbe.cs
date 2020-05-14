using System;
using UnityEngine;

namespace AmbientProbes
{
    [Serializable]
    public class AmbientProbe
    {
        public Vector3 position;
        /// <summary>
        /// When set, the probe takes on the RenderSettings ambient color
        /// </summary>
        public bool global;
        /// <summary>
        /// When enabled, a specific color can be set, otherwise the group's color is used
        /// </summary>
        public bool overriden;
        public Color color = Color.white;
        [Range(0f, 1f)]
        public float occlusion = 1f;    
    }
}