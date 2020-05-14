using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace AmbientProbes
{
    public class AmbientProbeSystem
    {
        public static List<AmbientProbeGroup> groups = new List<AmbientProbeGroup>();

        public static void Register(AmbientProbeGroup group)
        {
            if (groups.Contains(group) == false) groups.Add(group);
        }

        public static void Unregister(AmbientProbeGroup group)
        {
            if (groups.Contains(group)) groups.Remove(group);
        }

        /// <summary>
        /// Front end function to get ambient color at any position in world-space
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Color GetInterpolatedColor(Vector3 position)
        {
            Profiler.BeginSample("Ambient Probes: GetInterpolatedColor");
            ProbeInterpolation data = GetInterpolationData(position);

            if (!data.group) return RenderSettings.ambientSkyColor;

            //If one of the other is null
            if (data.dest == null && data.source != null) return data.source.color * data.source.occlusion;
            if (data.source == null && data.dest != null) return data.dest.color * data.dest.occlusion;
            if (data.source == null && data.dest == null) return RenderSettings.ambientSkyColor;

            Color src = data.source.overriden ? data.source.color : data.group.color;
            if (data.source.global) src = RenderSettings.ambientSkyColor;

            Color dst = data.dest.overriden ? data.dest.color : data.group.color;
            if (data.dest.global) dst = RenderSettings.ambientSkyColor;

            //Lerp
            Color ambient = Color.Lerp(dst, src, data.dist01);
            float occlusion = Mathf.Lerp(data.dest.occlusion, data.source.occlusion, data.dist01);

            Profiler.EndSample();

            return ambient * occlusion;
        }

        public struct ProbeInterpolation
        {
            public AmbientProbeGroup group;
            public AmbientProbe source;
            public AmbientProbe dest;
            public float dist01;
        }

        /// <summary>
        /// Checks if the position falls within the bounds of any groups
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static AmbientProbeGroup GetNearestGroup(Vector3 position)
        {
            foreach (AmbientProbeGroup group in groups)
            {
                if (group.bounds.Contains(position)) return group;
            }

            return null;
        }

        /// <summary>
        /// Return a struct with the nearest and 2nd nearest probe if the position is within any group's bounds
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static ProbeInterpolation GetInterpolationData(Vector3 position)
        {
            ProbeInterpolation data = new ProbeInterpolation();

            AmbientProbeGroup nearestGroup = GetNearestGroup(position);
            if (!nearestGroup) return data;

            data.group = nearestGroup;

            data.source = GetNearestProbe(nearestGroup, position);
            data.dest = GetNearestProbe(nearestGroup, position, data.source);

            //Distance value can exceed 1, but this is fine, the lerp will still return the destination
            if (data.source != null && data.dest != null) data.dist01 = (data.dest.position - position).magnitude / (data.dest.position - data.source.position).magnitude;
            data.dist01 = Mathf.Abs(data.dist01);

            return data;
        }

        //Note: Groups shouldn't have too many probes, otherwise some form of space partitioning is required!
        private static AmbientProbe GetNearestProbe(AmbientProbeGroup group, Vector3 position, AmbientProbe ignore = null)
        {
            AmbientProbe nearest = null;

            float minDist = 99999;
            for (int i = 0; i < group.probes.Count; i++)
            {
                //Assign the already closest probe to ignore, so we can get the 2nd clostest
                if (ignore != null && group.probes[i] == ignore) continue;

                if ((position - group.probes[i].position).sqrMagnitude < minDist)
                {
                    nearest = group.probes[i];
                    minDist = (position - group.probes[i].position).sqrMagnitude;
                }
            }

            return nearest;
        }
    }
}