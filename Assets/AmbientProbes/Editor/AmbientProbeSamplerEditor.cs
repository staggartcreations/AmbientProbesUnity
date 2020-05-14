using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmbientProbes
{
    [CustomEditor(typeof(AmbientProbeSampler))]
    public class AmbientProbeSamplerEditor : Editor
    {
        AmbientProbeSampler script;

        private void OnEnable()
        {
            script = (AmbientProbeSampler)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Sampled color" + (script.color == RenderSettings.ambientSkyColor ? " (Global)" : ""), EditorStyles.boldLabel);
 
            Rect rect = EditorGUILayout.GetControlRect();

            EditorGUI.DrawRect(rect, script.color);
        }
    }
}