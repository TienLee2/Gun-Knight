#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace MegaWorld.Edit
{
    public class MouseModifyGameObject
    {
        private InstanceData _instanceData;

        public void DoTool()
        {
            Event e = Event.current;
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            if(EditTool.FindGameObjectInfo == null)
            {
                HandleButton(Utility.GetRaycastMousePosition(edit.GroundLayers));
            }
            else
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    MegaWorldPath.GeneralDataPackage.StorageCells.RepositionObject(EditTool.FindGameObjectInfo.Obj, EditTool.FindGameObjectInfo.Proto.ID, EditTool.FindGameObjectInfo.CellIndex);
                    EditTool.FindGameObjectInfo = null;
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
                            axis = _instanceData.rotation * axis;
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

                EditTool.FindGameObjectInfo.Obj.transform.position = _instanceData.position;
                EditTool.FindGameObjectInfo.Obj.transform.rotation = _instanceData.rotation;
                EditTool.FindGameObjectInfo.Obj.transform.localScale = _instanceData.scale;
            }
        }

        private void HandleButton(Vector3 centerSpherePosition)
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            Bounds intersectBounds = new Bounds(centerSpherePosition, new Vector3(edit.SphereSize, edit.SphereSize, edit.SphereSize));

            MegaWorldPath.GeneralDataPackage.StorageCells.IntersectBounds(intersectBounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = AllAvailableTypes.GetCurrentPrototypeGameObject(gameObjectInfo.ID);

                if(proto == null)
                {
                    return true;
                }

                if(proto.selected == false || proto.active == false)
                {
                    return true;
                }

                for (int itemIndex = 0; itemIndex < gameObjectInfo.itemList.Count; itemIndex++)
                {
                    GameObject go = gameObjectInfo.itemList[itemIndex];

                    if (go == null)
                    {
                        continue;
                    }

                    GameObject prefabRoot = Utility.GetPrefabRoot(go);
                    if (prefabRoot == null)
                    {
                        continue;
                    }

                    Vector3 sceneCameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                    float distanceFromSceneCamera = Vector3.Distance(sceneCameraPosition, prefabRoot.transform.position);
                    float distanceFromCenterSphere = Vector3.Distance(centerSpherePosition, prefabRoot.transform.position);

                    if(distanceFromSceneCamera > edit.MaxDistance)
                    {
                        continue;
                    }

                    if(distanceFromCenterSphere > edit.SphereSize)
                    {
                        continue;
                    }

                    if(VladislavTsurikov.DrawHandles.HandleButton(EditTool.EditToolHash + go.GetHashCode(), go.transform.position, new Color(0.5f, 0.5f, 0.5f, 0.7f), new Color(1f, 1f, 0f, 0.7f)))
                    {
                        SetFindObject(go, proto, cellIndex);
                        return false;
                    }
                }

                return true;
            });
        }

        public void SetFindObject(GameObject obj, PrototypeGameObject proto, int cellIndex)
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;  

            EditTool.FindGameObjectInfo = new GameObjectInfo(obj, proto, cellIndex);

            _instanceData = new InstanceData(obj.transform.position, obj.transform.localScale, obj.transform.rotation, 1);

            edit.ObjectMouseModify.Prepare(_instanceData);
        }
    }
}
#endif