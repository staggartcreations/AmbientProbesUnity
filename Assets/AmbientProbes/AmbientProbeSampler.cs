using UnityEngine;

namespace AmbientProbes
{
    /// <summary>
    /// Samples ambient probes and applies to the color to its renderers through a MaterialPropertyBlock
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Rendering/Ambient Probes/Sampler")]
    public class AmbientProbeSampler : MonoBehaviour
    {
        public Renderer[] renderers;
        [Tooltip("The color property that should be set in the shader")]
        public string propertyName = "_Color";

        private MaterialPropertyBlock props;

        [HideInInspector]
        public Color color;

        private void OnEnable()
        {
            if (props == null) props = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (renderers == null) return;

            color = AmbientProbeSystem.GetInterpolatedColor(this.transform.position);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                {
                    renderers[i].GetPropertyBlock(props);

                    props.SetFloat("_AmbientProbes", 1); //Enable sampling in character shader
                    props.SetColor(propertyName, color);

                    renderers[i].SetPropertyBlock(props);
                }
            }
        }
    }
}