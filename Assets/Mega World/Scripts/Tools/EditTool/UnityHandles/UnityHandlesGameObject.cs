#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using QuadroRendererSystem;
using UnityEditor;

namespace MegaWorld.Edit
{
    public class UnityHandlesGameObject
    {
        public Vector3 pastCenterSpherePosition;

        public void DoTool()
        {
            Event e = Event.current;
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            if(EditTool.FindGameObjectInfo == null)
            {
                Handles(Utility.GetRaycastMousePosition(edit.GroundLayers), true);
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

                Handles(pastCenterSpherePosition, false);
            }
        }

        private void Handles(Vector3 centerSpherePosition, bool findObject)
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

                    if(EditTool.FindGameObjectInfo == null)
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
                        if(EditTool.FindGameObjectInfo.Obj != prefabRoot)
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

                    InstanceData instanceData = new InstanceData(prefabRoot.transform.position, prefabRoot.transform.localScale, prefabRoot.transform.rotation, 1);
                    
                    if(UnityHandles.DrawModifyHandles(instanceData))
                    {
                        prefabRoot.transform.position = instanceData.position;
                        prefabRoot.transform.rotation = instanceData.rotation;
                        prefabRoot.transform.localScale = instanceData.scale;

                        if(findObject)
                        {
                            SetFindObject(go, proto, cellIndex);
                            pastCenterSpherePosition = centerSpherePosition;
                            return false;
                        }
                    }
                }

                return true;
            });
        }

        public void SetFindObject(GameObject go, PrototypeGameObject proto, int cellIndex)
        {
            EditTool.FindGameObjectInfo = new GameObjectInfo(go, proto, cellIndex);
        }
    }
}
#endif