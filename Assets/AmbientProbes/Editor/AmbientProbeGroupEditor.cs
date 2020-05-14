using UnityEditor;
using UnityEngine;

namespace AmbientProbes
{
    [CustomEditor(typeof(AmbientProbeGroup))]
    public class AmbientProbeGroupEditor : Editor
    {
        AmbientProbeGroup script;
        public static AmbientProbeGroup selected;

        private void OnEnable()
        {
            script = (AmbientProbeGroup)target;
            selected = script;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Edit probes", EditorGUIUtility.IconContent("LightProbeProxyVolume Gizmo").image), GUILayout.MaxWidth(125f), GUILayout.MaxHeight(30f)))
            {
                selected = script;
                AmbientProbesEditor.OpenWindow();
            }

            EditorGUILayout.Space();

            base.OnInspectorGUI();

        }
    }
}