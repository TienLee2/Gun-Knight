#if UNITY_EDITOR
using UnityEngine;
using System;
using QuadroRendererSystem;
using UnityEditor;

namespace MegaWorld.Edit
{
    public class UnityHandlesQuadroItem
    {
        public Vector3 PastCenterSpherePosition;

        public void DoTool()
        {
            Event e = Event.current;
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            if(EditTool.FindQuadroItemInfo == null)
            {
                Handles(Utility.GetRaycastMousePosition(edit.GroundLayers), true);
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

                Handles(PastCenterSpherePosition, false);
            }
        }

        private void Handles(Vector3 centerSpherePosition, bool findObject)
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

                    if(EditTool.FindQuadroItemInfo == null)
                    {
                        if(distanceFromSceneCamera > edit.MaxDistance)
                        {
                            continue;
                        }

                        if(distanceFromCenterSphere > edit.SphereSize)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if(EditTool.FindQuadroItemInfo.Obj != objectInstanced)
                        {
                            if(distanceFromSceneCamera > edit.MaxDistance)
                            {
                                continue;
                            }

                            if(distanceFromCenterSphere > edit.SphereSize)
                            {
                                continue;
                            }
                        }
                    }

                    InstanceData instanceData = new InstanceData(objectInstanced.Position, objectInstanced.Scale, objectInstanced.Rotation, 1);
                    
                    if(UnityHandles.DrawModifyHandles(instanceData))
                    {
                        objectInstanced.Position = instanceData.position;
                        objectInstanced.Rotation = instanceData.rotation;
                        objectInstanced.Scale = instanceData.scale;

                        if(findObject)
                        {
                            SetFindObject(objectInstanced, proto, cellIndex);
                            PastCenterSpherePosition = centerSpherePosition;
                            return false;
                        }
                    }
                }

                return true;
            });
        }

        public void SetFindObject(ObjectInstanceData obj, PrototypeQuadroItem proto, int cellIndex)
        {
            EditTool.FindQuadroItemInfo = new QuadroItemInfo(obj, proto, cellIndex);
        }
    }
}
#endif