using System;
using UnityEngine;
using UnityEditor;

namespace MegaWorld
{
    public class GameObjectUtility : MonoBehaviour
    {
        public static bool GetObjectWorldBounds(GameObject gameObject, out Bounds bounds)
        {
            Bounds worldBounds = new Bounds();
            bool found = false;

            Utility.ForAllInHierarchy(gameObject, (go) =>
            {
                if (!go.activeInHierarchy)
                    return;

                Renderer renderer = go.GetComponent<Renderer>();
                SkinnedMeshRenderer skinnedMeshRenderer;
                RectTransform rectTransform;

                if (renderer != null)
                {
                    if (!found)
                    {
                        worldBounds = renderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(renderer.bounds);
                    }
                }
                else if ((skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>()) != null)
                {
                    if (!found)
                    {
                        worldBounds = skinnedMeshRenderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(skinnedMeshRenderer.bounds);
                    }
                }
                else if ((rectTransform = go.GetComponent<RectTransform>()) != null)
                {
                    Vector3[] fourCorners = new Vector3[4];
                    rectTransform.GetWorldCorners(fourCorners);
                    Bounds rectBounds = new Bounds();

                    rectBounds.center = fourCorners[0];
                    rectBounds.Encapsulate(fourCorners[1]);
                    rectBounds.Encapsulate(fourCorners[2]);
                    rectBounds.Encapsulate(fourCorners[3]);

                    if (!found)
                    {
                        worldBounds = rectBounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(rectBounds);
                    }
                }
             });

            if (!found)
                bounds = new Bounds(gameObject.transform.position, Vector3.one);
            else
                bounds = worldBounds;

            return found;
        }

        public static PlacedObjectInfo GetPlacedObjectInfo(GameObject gameObject)
        {
            PlacedObjectInfo obj = new PlacedObjectInfo();

            obj.gameObject = gameObject;

            GetObjectWorldBounds(gameObject, out obj.bounds);

            return obj;
        }

#if UNITY_EDITOR
        public static void SetFlags(FlagsSettings flagsSettings, GameObject go)
        {
            if (go == null)
            {
                return;
            }

            StaticEditorFlags flags = 0;

            switch (flagsSettings.TagsSelectedType)
            {
                case TagsSelectedType.Everything:
                {
                    flags |= true ? StaticEditorFlags.BatchingStatic : flags;
                    flags |= true ? StaticEditorFlags.OccludeeStatic : flags;
                    flags |= true ? StaticEditorFlags.OccluderStatic : flags;
                    #if UNITY_2019_2_OR_NEWER
                    flags |= true ? StaticEditorFlags.ContributeGI : flags;
                    #endif
                    flags |= true ? StaticEditorFlags.NavigationStatic : flags;
                    flags |= true ? StaticEditorFlags.OffMeshLinkGeneration : flags;
                    flags |= true ? StaticEditorFlags.ReflectionProbeStatic : flags;
                    break;
                }
                case TagsSelectedType.Custom:
                {
                    flags |= flagsSettings.BatchingStatic ? StaticEditorFlags.BatchingStatic : flags;
                    flags |= flagsSettings.OccludeeStatic ? StaticEditorFlags.OccludeeStatic : flags;
                    flags |= flagsSettings.OccluderStatic ? StaticEditorFlags.OccluderStatic : flags;
                    #if UNITY_2019_2_OR_NEWER
                    flags |= flagsSettings.LightmapStatic ? StaticEditorFlags.ContributeGI : flags;
                    #endif
                    flags |= flagsSettings.NavigationStatic ? StaticEditorFlags.NavigationStatic : flags;
                    flags |= flagsSettings.OffMeshLinkGeneration ? StaticEditorFlags.OffMeshLinkGeneration : flags;
                    flags |= flagsSettings.ReflectionProbeStatic ? StaticEditorFlags.ReflectionProbeStatic : flags;
                    break;
                }
            }

            UnityEditor.GameObjectUtility.SetStaticEditorFlags(go, flags);
        }
#endif

        public static void SetInstanceDataForGameObject(ref InstanceData spawnInfo, PrototypeGameObject proto, Vector3 normal)
        {
            if(proto.TransformComponentsStack != null)
            {      
                foreach (TransformComponent item in proto.TransformComponentsStack.TransformComponents)
                {
                    if(item.Enabled)
                    {
                        item.SetInstanceData(ref spawnInfo, normal);
                    }
                }
            }
        }

        private static void SetInstanceDataForQuadroItem(ref InstanceData spawnInfo, PrototypeQuadroItem proto, Vector3 normal)
        {
            if(proto.TransformComponentsStack != null)
            {      
                foreach (TransformComponent item in proto.TransformComponentsStack.TransformComponents)
                {
                    if(item.Enabled)
                    {
                        item.SetInstanceData(ref spawnInfo, normal);
                    }
                }
            }
        }

        public static PlacedObjectInfo PlaceObject(PrototypeGameObject proto, Vector3 position, Vector3 scaleFactor, Quaternion rotation)
        {
            GameObject go;

#if UNITY_EDITOR
            go = PrefabUtility.InstantiatePrefab(proto.prefab) as GameObject;
#else
            go = Instantiate(proto.prefab);
#endif

            go.transform.position = position;
            go.transform.localScale = scaleFactor;
            go.transform.rotation = rotation;

#if UNITY_EDITOR
            SetFlags(proto.FlagsSettings, go);
#endif
            PlacedObjectInfo objectInfo = GetPlacedObjectInfo(go);

            if(MegaWorldPath.GeneralDataPackage.EnableUndoForGameObject)
            {   
#if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(go, "PP: Paint Prefabs");
#endif
            }

            return objectInfo;
        }

        public static void FindTypeParentObject(Type type)
        {
            string typeGroupName = type.TypeName;

            Transform typeGroup = null;
            
            GameObject[] sceneRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject root in sceneRoots)
			{
				if(root.name == typeGroupName) 
                {
					typeGroup = root.transform;
                    break;
				}
			} 

            if (typeGroup == null)
            {
                GameObject childObject = new GameObject(typeGroupName);
                typeGroup = childObject.transform;
            }

            type.TypeParentObject = typeGroup.gameObject;
        }

        public static void ParentGameObject(Type type, PrototypeGameObject proto, PlacedObjectInfo objectInfo)
        {            
            if(type.TypeParentObject == null)
            {
                FindTypeParentObject(type);
            }

            if(type.TypeParentObject != null)
            {
                GameObject typeParentObject = type.TypeParentObject;

                if (objectInfo.gameObject.gameObject != null && typeParentObject != null && typeParentObject.transform != null)
                {
                    objectInfo.gameObject.transform.SetParent(typeParentObject.transform, true);
                }
            }
        }

        public static void GetRightForward(Vector3 up, out Vector3 right, out Vector3 forward)
        {
            forward = Vector3.Cross(Vector3.right, up).normalized;
            if (forward.magnitude < 0.001f)
                forward = Vector3.forward;

            right = Vector3.Cross(up, forward).normalized;
        }

        public static void GetOrientation(Vector3 normal, FromDirection mode, float weightToNormal, out Vector3 upwards, out Vector3 right, out Vector3 forward)
        {
            switch (mode)
            {
                case FromDirection.SurfaceNormal:
                    upwards = Vector3.Lerp(Vector3.up, normal, weightToNormal);
                    break;
                case FromDirection.X:
                    upwards = new Vector3(1, 0, 0);
                    break;
                default:
                case FromDirection.Y:
                    upwards = new Vector3(0, 1, 0);
                    break;
                case FromDirection.Z:
                    upwards = new Vector3(0, 0, 1);
                    break;
            }

            GetRightForward(upwards, out right, out forward);
        }
    }
}