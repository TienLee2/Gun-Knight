using QuadroRendererSystem;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov;

namespace MegaWorld
{
    public enum SynchronizationError
    {
        None,
        QuadroRendererNull, 
        StorageTerrainCellsNull,
        NotAllProtoAvailable
    }

    public class QuadroRendererController : MonoBehaviour
    {
#if UNITY_EDITOR
        public static bool QuadroRendererControllerFoldout = true;
        public static QuadroRendererControllerEditor QuadroRendererControllerEditor = new QuadroRendererControllerEditor();
#endif

        public static SynchronizationError SynchronizationError = SynchronizationError.None;

        private static QuadroRenderer s_quadroRenderer;
        public static QuadroRenderer QuadroRenderer
        {
            get
            {
                if(s_quadroRenderer == null)
                {
                    s_quadroRenderer = (QuadroRenderer)FindObjectOfType(typeof(QuadroRenderer));
                }
                
                return s_quadroRenderer;

            }
            set
            {
                s_quadroRenderer = value;
            }
        }

        private static StorageTerrainCells s_storageTerrainCells;
        public static StorageTerrainCells StorageTerrainCells
        {
            get
            {
                if(s_storageTerrainCells == null)
                {
                    s_storageTerrainCells = (StorageTerrainCells)FindObjectOfType(typeof(StorageTerrainCells));
                }
                
                return s_storageTerrainCells;
            }
            set
            {
                s_storageTerrainCells = value;
            }
        }

        public static void CreateQuadroRenderer()
        {
            #if UNITY_EDITOR
            MegaWorldGUIUtility.CallMenu("GameObject/Quadro Renderer/Add Quadro Renderer");
            #endif

            SetQuadroRendererInfo();
        }

        public static void AddStorageTerrainCells()
        {
            StorageTerrainCells storageTerrainCells = QuadroRenderer.gameObject.AddComponent<StorageTerrainCells>();
			storageTerrainCells.CreateCells();

            QuadroRenderer.DetectAdditionalData();
        }

        public static void SetQuadroRendererInfo()
        {
            s_quadroRenderer = (QuadroRenderer)FindObjectOfType(typeof(QuadroRenderer));
            s_storageTerrainCells = (StorageTerrainCells)FindObjectOfType(typeof(StorageTerrainCells));
        }

        public static void UpdateQuadroRenderer(Type type)
        {
            List<PrototypeQuadroItem> protoQuadroItemRemoveList = new List<PrototypeQuadroItem>();

            List<QuadroPrototype> quadroPrototypeList = new List<QuadroPrototype>(s_quadroRenderer.QuadroPrototypesPackage.PrototypeList);

            foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
            {
                bool find = false;

                for (int Id = 0; Id < quadroPrototypeList.Count; Id++)
                {
                    if (Utility.IsSameGameObject(quadroPrototypeList[Id].PrefabObject, proto.prefab, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    protoQuadroItemRemoveList.Add(proto);
                }
            }

            foreach (PrototypeQuadroItem proto in protoQuadroItemRemoveList)
            {
                type.ProtoQuadroItemList.Remove(proto);
            }

            QuadroPrototype quadroPrototype;
            PrototypeQuadroItem prototypeQuadroItem;

            for (int Id = 0; Id < quadroPrototypeList.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
                {
                    if (Utility.IsSameGameObject(quadroPrototypeList[Id].PrefabObject, proto.prefab, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    quadroPrototype = quadroPrototypeList[Id];

                    prototypeQuadroItem = new PrototypeQuadroItem(quadroPrototype.PrefabObject, quadroPrototype.Bounds.size / 2);
    
                    type.ProtoQuadroItemList.Add(prototypeQuadroItem);
                }
            }
        }

        public static bool IsSyncID(Type type)
        {
            foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
            {
                if(QuadroRendererController.s_quadroRenderer.QuadroPrototypesPackage.GetQuadroItem(proto.ID) == null)
                {
                    if(IsAllPrefabAvailable(type) == false)
                    {
                        return false;
                    }
                    else
                    {
                        SyncID(type);
                    }
                }
            }

            return true;
        }

        public static bool IsAllPrefabAvailable(Type type)
        {
            foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
            {
                if(QuadroRendererController.s_quadroRenderer.QuadroPrototypesPackage.GetQuadroItem(proto.prefab) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static void SyncID(Type type)
        {
            foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
            {
                if(QuadroRendererController.s_quadroRenderer.QuadroPrototypesPackage.GetQuadroItem(proto.prefab) != null)
                {
                    proto.ID = QuadroRendererController.s_quadroRenderer.QuadroPrototypesPackage.GetQuadroItem(proto.prefab).ID;
                }
            }
        }

        public static void AddQuadroItem(List<PrototypeQuadroItem> protoQuadroItemList)
        {
            if(CheckForMissingOfData() == false)
            {
                return;
            }

            foreach (PrototypeQuadroItem protoQuadroItem in protoQuadroItemList)
            {
                QuadroPrototype quadroPrototype = QuadroRenderer.QuadroPrototypesPackage.GetQuadroItem(protoQuadroItem.ID);

                if(quadroPrototype == null)
                {
                    QuadroRenderer.QuadroPrototypesPackage.AddPrototype(s_quadroRenderer, protoQuadroItem.prefab, s_quadroRenderer.QuadroRendererCamera.Count, protoQuadroItem.ID);
                }
            }

            RefreshQuadroRenderer();
        }

        public static void RemoveAllQuadroItem()
        {
            if(CheckForMissingOfData() == false)
            {
                return;
            }

            foreach (QuadroPrototype proto in s_quadroRenderer.QuadroPrototypesPackage.PrototypeList)
            {
                s_quadroRenderer.QuadroPrototypesPackage.DeleteAssetPrototype(proto);
            }

            s_quadroRenderer.QuadroPrototypesPackage.PrototypeList.RemoveAll((proto) => proto == null);

            RefreshQuadroRenderer();
        }

        public static void AddItemToStorageTerrainCells(PrototypeQuadroItem proto, InstanceData instanceData)
        {
            if(CheckForMissingOfData() == false)
            {
                return;
            }

            StorageTerrainCellsAPI.AddItemInstance(s_storageTerrainCells, proto.ID, instanceData.position, instanceData.scale, instanceData.rotation);
        }

        public static void RefreshQuadroRenderer()
        {
            if(CheckForMissingOfData() == false)
            {
                return;
            }

            s_quadroRenderer.RefreshQuadroRenderer();
        }

        public static void SetCurrentSynchronizationError(Type type)
        {
            if(QuadroRendererController.QuadroRenderer == null)
            {
                SynchronizationError = SynchronizationError.QuadroRendererNull;
            }
			else if(QuadroRendererController.StorageTerrainCells == null)
			{
                SynchronizationError = SynchronizationError.StorageTerrainCellsNull;
			}
			else if(QuadroRendererController.IsSyncID(type) == false)
			{
                SynchronizationError = SynchronizationError.NotAllProtoAvailable;
            }
            else
            {
                SynchronizationError = SynchronizationError.None;
            }
        }

        public static bool SpawnSupportAvailable(Type type)
        {
            if(QuadroRendererController.QuadroRenderer == null)
            {
                Debug.LogWarning("There is no Quadro Renderer in the scene. Click the button \"Create Quadro Renderer\" in Quadro Renderer Controller in Mega World");
                return false;
            }
			else if(QuadroRendererController.StorageTerrainCells == null)
			{
                Debug.LogWarning("There is no Storage Terrain Cells in the scene. Click the button \"Add Storage Terrain Cells\"");
                return false;
			}
			else if(QuadroRendererController.IsSyncID(type) == false)
			{
                Debug.LogWarning("You need all prototypes of this type (" + type.TypeName + ") to be in Quadro Renderer.");
                return false;
            }

            return true;
        }

        public static bool CheckForMissingOfData()
        {
            if(s_storageTerrainCells == null)
            {
                SetQuadroRendererInfo();
                return false;
            }
            if(s_quadroRenderer == null)
            {
                SetQuadroRendererInfo();
                return false;
            }

            return true;
        }
    }
}