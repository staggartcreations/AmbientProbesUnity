using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmbientProbes
{
    public class AmbientProbesEditor : EditorWindow
    {
        public static AmbientProbesEditor Instance;

        public static void OpenWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(AmbientProbesEditor), false);
            window.titleContent = new GUIContent("Ambient Probes");
            window.autoRepaintOnSceneChange = true;
        }

        private void OnEnable()
        {
            Instance = this;

            GetSelected();
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        private void OnSelectionChange()
        {
            GetSelected();

            Repaint();
        }

        private static void GetSelected()
        {
            if (!Selection.activeGameObject)
            {
                selectedSampler = null;
                return;
            }

            selectedSampler = Selection.activeGameObject.GetComponent<AmbientProbeSampler>();
        }

        private static AmbientProbeSampler selectedSampler;
        private static AmbientProbe source;
        private static AmbientProbe dest;
        private static float dist;

        private static AmbientProbe selected;
        private static void OnSceneGUI(SceneView sceneView)
        {
            AmbientProbesEditor.Instance.Repaint();

            if (AmbientProbeGroupEditor.selected)
            {
                Handles.color = new Color(1, 1, 1, 0.25f);
                Handles.DrawWireCube(AmbientProbeGroupEditor.selected.bounds.center, AmbientProbeGroupEditor.selected.bounds.size);

                foreach (AmbientProbe probe in AmbientProbeGroupEditor.selected.probes)
                {
                    //Set to color of probe
                    Handles.color = (probe.overriden ? probe.color : AmbientProbeGroupEditor.selected.color);
                    if (probe.global) Handles.color = RenderSettings.ambientSkyColor;

                    Handles.color *= probe.occlusion;
                    Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, 1f);

                    //Handles.SphereHandleCap(0, probe.position, Quaternion.identity, 0.5f, EventType.Repaint);
                    Handles.DrawSolidDisc(probe.position, sceneView.camera.transform.forward, 0.25f);
                    if(selected == probe) Handles.DrawWireDisc(probe.position, sceneView.camera.transform.forward, 0.3f);

                    if (Handles.Button(probe.position, Quaternion.identity, 0f, 0.25f, Handles.SphereHandleCap))
                    {
                        selected = probe;
                    }

                    if (probe.global)
                    {
                        Handles.Label(probe.position, EditorGUIUtility.IconContent("PreTextureAlpha"));
                    }
                    if (probe.overriden)
                    {
                        Handles.Label(probe.position, EditorGUIUtility.IconContent("PreTextureRGB"));
                    }
                }

                EditorGUI.BeginChangeCheck();

                if (selected != null)
                {
                    if (Tools.current == Tool.Move)
                    {
                        selected.position = Handles.PositionHandle(selected.position, Quaternion.identity);
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(AmbientProbeGroupEditor.selected);
                    AmbientProbeGroupEditor.selected.RecalculateBounds();
                }
            }

            if (selectedSampler)
            {
                AmbientProbeSystem.ProbeInterpolation data = AmbientProbeSystem.GetInterpolationData(selectedSampler.transform.position);

                source = data.source;
                dest = data.dest;
                dist = data.dist01;

                Handles.color = Color.white;

                if (source != null) Handles.DrawAAPolyLine(Texture2D.whiteTexture, 2f, new Vector3[] { selectedSampler.transform.position, source.position });
                if (dest != null) Handles.DrawAAPolyLine(Texture2D.whiteTexture, 2f, new Vector3[] { selectedSampler.transform.position, dest.position });

                Handles.Label(selectedSampler.transform.position, new GUIContent("Distance: " + dist));
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Selected group: " + (!AmbientProbeGroupEditor.selected ? "NONE" : AmbientProbeGroupEditor.selected.name));

            if (!AmbientProbeGroupEditor.selected)
            {
                foreach (AmbientProbeGroup group in AmbientProbeSystem.groups)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(group.name);
                        if (GUILayout.Button("Select"))
                        {
                            AmbientProbeGroupEditor.selected = group;
                        }
                    }
                }
                return;
            }

            EditorGUI.BeginChangeCheck();
            AmbientProbeGroupEditor.selected.color = EditorGUILayout.ColorField("Color", AmbientProbeGroupEditor.selected.color);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(AmbientProbeGroupEditor.selected);
            }
            EditorGUILayout.LabelField("Probe count: " + AmbientProbeGroupEditor.selected.probes.Count);

            if (GUILayout.Button("Add new"))
            {
                AmbientProbe probe = new AmbientProbe();
                probe.position = SceneView.lastActiveSceneView.camera.transform.position + (SceneView.lastActiveSceneView.camera.transform.forward * 10f);

                AmbientProbeGroupEditor.selected.probes.Add(probe);
                AmbientProbeGroupEditor.selected.RecalculateBounds();
                EditorUtility.SetDirty(AmbientProbeGroupEditor.selected);
            }

            if (selectedSampler) EditorGUILayout.LabelField("Nearest group: " + AmbientProbeSystem.GetNearestGroup(selectedSampler.transform.position));

                EditorGUILayout.LabelField("Selected", EditorStyles.boldLabel);
            if (selected != null)
            {

                EditorGUI.BeginChangeCheck();

                selected.global = EditorGUILayout.Toggle("Global", selected.global);
                if (!selected.global) selected.overriden = EditorGUILayout.Toggle("Override", selected.overriden);
                if (!selected.global && selected.overriden) selected.color = EditorGUILayout.ColorField("Color", selected.color);
                selected.occlusion = EditorGUILayout.Slider("Occlusion", selected.occlusion, 0f, 1f);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(AmbientProbeGroupEditor.selected);
                }

                using (new EditorGUILayout.HorizontalScope())
                {

                    if (GUILayout.Button("Duplicate"))
                    {
                        AmbientProbe probe = new AmbientProbe();

                        probe.position = selected.position + Vector3.right;
                        probe.global = selected.global;
                        probe.overriden = selected.overriden;
                        probe.color = selected.color;
                        probe.occlusion = selected.occlusion;

                        AmbientProbeGroupEditor.selected.probes.Add(probe);
                        EditorUtility.SetDirty(AmbientProbeGroupEditor.selected);

                        selected = probe;
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        AmbientProbeGroupEditor.selected.probes.Remove(selected);
                        selected = null;
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("None", EditorStyles.miniLabel);
            }
        }


    }
}