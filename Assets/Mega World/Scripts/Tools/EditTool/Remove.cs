using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using QuadroRendererSystem;

namespace MegaWorld.Edit
{
#if UNITY_EDITOR
    public class Remove : MonoBehaviour
    {
        public static void RemoveHandles()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
            {
                RemoveGameObject();
            }
            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList.Count != 0)
            {
                RemoveQuadroItem();
            }
        }

        public static void RemoveGameObject()
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            Vector3 centerSpherePosition = Utility.GetRaycastMousePosition(edit.GroundLayers);

            Bounds intersectBounds = new Bounds(centerSpherePosition, new Vector3(edit.SphereSize, edit.SphereSize, edit.SphereSize));

            MegaWorldPath.GeneralDataPackage.StorageCells.IntersectBounds(intersectBounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = AllAvailableTypes.GetCurrentPrototypeGameObject(gameObjectInfo.ID);

                if(proto == null || proto.active == false)
                {
                    return true;
                }

                if(proto.selected == false)
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

                    if(VladislavTsurikov.DrawHandles.HandleButton(EditTool.EditToolHash + go.GetHashCode(), prefabRoot.transform.position, new Color(1f, 0f, 0f, 0.7f), new Color(1f, 1f, 0f, 0.7f)))
                    {
                        DestroyImmediate(prefabRoot);
                        MegaWorldPath.GeneralDataPackage.StorageCells.cellList[cellIndex].RemoveNullGameObjects();
                        return false;
                    }
                }

                return true;
            });
        }

        public static void RemoveQuadroItem()
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            Vector3 centerSpherePosition = Utility.GetRaycastMousePosition(edit.GroundLayers);

            Bounds intersectBounds = new Bounds(centerSpherePosition, new Vector3(edit.SphereSize, edit.SphereSize, edit.SphereSize));

            StorageTerrainCellsAPI.IntersectBounds(QuadroRendererController.StorageTerrainCells, intersectBounds, true, false, (persistentInfo, cellIndex) =>
            {
                PrototypeQuadroItem proto = AllAvailableTypes.GetCurrentQuadroItem(persistentInfo.ID); 

                if(proto == null || proto.active == false)
                {
                    return true;
                }

                if(proto.selected == false)
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

                    if(VladislavTsurikov.DrawHandles.HandleButton(EditTool.EditToolHash + objectInstanced.GetHashCode(), objectInstanced.Position, new Color(1f, 0f, 0f, 0.7f), new Color(1f, 1f, 0f, 0.7f)))
                    {
                        persistentInfo.ObjectInstanceData.Remove(objectInstanced);
                        QuadroRendererController.QuadroRenderer.StorageTerrainCells.CellModifier.RemoveAfterConvert = true;
                        return false;
                    }
                }

                return true;
            });
        }
    }
#endif
}