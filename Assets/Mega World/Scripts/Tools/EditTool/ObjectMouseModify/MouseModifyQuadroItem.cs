#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using QuadroRendererSystem;

namespace MegaWorld.Edit
{
    public class MouseModifyQuadroItem
    {
        private InstanceData instanceData;

        public void DoTool()
        {
            Event e = Event.current;
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            if(EditTool.FindQuadroItemInfo == null)
            {
                HandleButton(Utility.GetRaycastMousePosition(edit.GroundLayers));
            }
            else
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    StorageTerrainCellsAPI.RepositionObject(QuadroRendererController.StorageTerrainCells, EditTool.FindQuadroItemInfo.Obj, EditTool.FindQuadroItemInfo.Proto.ID, EditTool.FindQuadroItemInfo.CellIndex);
                    EditTool.FindQuadroItemInfo = null;
                    return;
                }

                MouseCursor.Instance.HandleMouseMoveEvent(e);

                switch (edit.PrecisionModifyMode)
                {
                    case PrecisionModifyMode.Position:
                    {
                        edit.ObjectMouseModify.PositionObject(Vector3.up);

                        break;
                    }
                    case PrecisionModifyMode.Rotation:
                    {
                        Vector3 axis = Vector3.up;
                        if(MegaWorldPath.GeneralDataPackage.TransformSpace == TransformSpace.Local)
                        {
                            axis = instanceData.rotation * axis;
                        }

                        edit.ObjectMouseModify.RotateObject(axis);

                        break;
                    }
                    case PrecisionModifyMode.Scale:
                    {
                        edit.ObjectMouseModify.ScaleObject();

                        break;
                    }
                }

                EditTool.FindQuadroItemInfo.Obj.Position = instanceData.position;
                EditTool.FindQuadroItemInfo.Obj.Rotation = instanceData.rotation.normalized;
                EditTool.FindQuadroItemInfo.Obj.Scale = instanceData.scale;
            }
        }

        private void HandleButton(Vector3 centerSpherePosition)
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            Bounds intersectBounds = new Bounds(centerSpherePosition, new Vector3(edit.SphereSize, edit.SphereSize, edit.SphereSize));

            StorageTerrainCellsAPI.IntersectBounds(QuadroRendererController.StorageTerrainCells, intersectBounds, true, false, (persistentInfo, cellIndex) =>
            {
                PrototypeQuadroItem proto = AllAvailableTypes.GetCurrentQuadroItem(persistentInfo.ID); 

                if(proto == null)
                {
                    return true;
                }

                if(proto.selected == false || proto.active == false)
                {
                    return true;
                }

                for (int itemIndex = 0; itemIndex < persistentInfo.ObjectInstanceData.Count; itemIndex++)
                {
                    ObjectInstanceData objectInstanced = persistentInfo.ObjectInstanceData[itemIndex];

                    Vector3 sceneCameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                    float distanceFromSceneCamera = Vector3.Distance(sceneCameraPosition, objectInstanced.Position);
                    float distanceFromCenterSphere = Vector3.Distance(centerSpherePosition, objectInstanced.Position);

                    if(distanceFromSceneCamera > edit.MaxDistance)
                    {
                        continue;
                    }

                    if(distanceFromCenterSphere > edit.SphereSize)
                    {
                        continue;
                    }
                    
                    if(VladislavTsurikov.DrawHandles.HandleButton(EditTool.EditToolHash + objectInstanced.GetHashCode(), objectInstanced.Position, new Color(0.5f, 0.5f, 0.5f, 0.7f), new Color(1f, 1f, 0f, 0.7f)))
                    {
                        SetFindObject(objectInstanced, proto, cellIndex);
                        return false;
                    }
                }

                return true;
            });
        }

        public void SetFindObject(ObjectInstanceData obj, PrototypeQuadroItem proto, int cellIndex)
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;  

            EditTool.FindQuadroItemInfo = new QuadroItemInfo(obj, proto, cellIndex);

            instanceData = new InstanceData(obj.Position, obj.Scale, obj.Rotation, 1);

            edit.ObjectMouseModify.Prepare(instanceData);
        }
    }
}
#endif