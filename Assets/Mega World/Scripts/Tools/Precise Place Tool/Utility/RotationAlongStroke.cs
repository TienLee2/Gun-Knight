using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.TerrainAPI;
using System.Collections.Generic;

namespace MegaWorld.PrecisePlace
{
    public static class RotationAlongStroke 
    {
        public static void RotateAlongStroke(GameObject gameObject, DragBrush drag)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            Vector3 upwards;
            Vector3 right;
            Vector3 forward;

            GameObjectUtility.GetOrientation(drag.raycast.normal, precisionPaintSettings.AlignAxis ? FromDirection.SurfaceNormal : FromDirection.Y, precisionPaintSettings.WeightToNormal,
                out upwards, out right, out forward);

            Vector3 strokeForward;

            strokeForward = Vector3.Cross(drag.strokeDirection, upwards);

            if (strokeForward.magnitude > 0.001f)
                forward = strokeForward;

            gameObject.transform.rotation = Quaternion.LookRotation(forward, upwards);
        }
    }
}