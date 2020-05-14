using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbientProbes
{
    [ExecuteInEditMode]
    [AddComponentMenu("Rendering/Ambient Probes/Group")]
    public class AmbientProbeGroup : MonoBehaviour
    {
        public Bounds bounds;
        public List<AmbientProbe> probes = new List<AmbientProbe>();
        [Tooltip("Color applies to any probes not overiding it")]
        public Color color = Color.white;

        private void OnEnable()
        {
            AmbientProbeSystem.Register(this);
        }

        private void OnDisable()
        {
            AmbientProbeSystem.Unregister(this);
        }

        public void RecalculateBounds()
        {
            bounds = new Bounds(this.transform.position, Vector3.zero);

            foreach (AmbientProbe probe in probes)
            {
                bounds.Encapsulate(probe.position);
            }

            //Padding
            bounds.Expand(3f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}