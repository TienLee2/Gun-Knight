using System;
using System.Reflection;
using UnityEngine;

namespace QuadroRendererSystem
{
    public static class GeometryUtilityAllocFree
    {
        public static Plane[] FrustrumPlanes = new Plane[6];

        private static readonly Action<Plane[], Matrix4x4> InternalExtractPlanes =
            (Action<Plane[], Matrix4x4>)Delegate.CreateDelegate(
                typeof(Action<Plane[], Matrix4x4>),
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(GeometryUtility).GetMethod("Internal_ExtractPlanes",
                    BindingFlags.Static | BindingFlags.NonPublic));

        public static void CalculateFrustrumPlanes(Camera camera)
        {
            InternalExtractPlanes(FrustrumPlanes, camera.projectionMatrix * camera.worldToCameraMatrix);
        }
    }
}
