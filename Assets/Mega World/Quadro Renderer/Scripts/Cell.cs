using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [Serializable]
    public class Cell : IHasRect
    {
        public Bounds Bounds;
        public RenderInstances RenderInstancesList;
        public List<ItemInfo> ItemInfoList = new List<ItemInfo>();
        public int Index;

        public Color DebugColor = Color.white;

        public Cell(Bounds bounds)
        {
            this.Bounds = bounds;
        }

        public Cell(Bounds bounds, Color debugColor)
        {
            this.Bounds = bounds;
            this.DebugColor = debugColor;
        }

        public Cell(Rect rectangle)
        {
            Bounds = RectExtension.CreateBoundsFromRect(rectangle, -100000);
        }

        public Rect Rectangle
        {
            get
            {
                return RectExtension.CreateRectFromBounds(Bounds);
            }
            set
            {
                Bounds = RectExtension.CreateBoundsFromRect(value);
            }
        }

        public void PrepareCell(QuadroRenderer quadroRenderer)
        {
            RenderInstancesList = new RenderInstances(quadroRenderer.QuadroPrototypesPackage.PrototypeList.Count);
        }

        public void AddItemInstance(int ID, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            ItemInfo persistentInfo = GetPersistentInfo(ID);
            if (persistentInfo == null)
            {
                persistentInfo = new ItemInfo 
                {
                    ID = ID
                };
                ItemInfoList.Add(persistentInfo);
            }

            InstanceData persistentItem = new InstanceData
            {
                Position = position,
                Rotation = rotation,
                Scale = scale,
            };

            persistentInfo.AddPersistentItemInstance(ref persistentItem);
        }

        public ItemInfo GetPersistentInfo(int ID)
        {
            for (int i = 0; i <= ItemInfoList.Count - 1; i++)
            {
                if (ItemInfoList[i].ID == ID) 
                {
                    return ItemInfoList[i];
                }
            }

            return null;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Bounds.center, Bounds.extents.magnitude);
        }

        public void DisposeUnmanagedData()
        {
            if(RenderInstancesList == null) 
            {
                return;
            }

            RenderInstancesList.DisposeUnmanagedData(); 
        }
   
        public void ClearInstanceMemory()
        {
            if(RenderInstancesList == null)
            {
                return;
            }

            RenderInstancesList.ClearInstanceMemory(); 
        }

        public void DisposePersistent()
        {           
            for (int i = 0; i <= ItemInfoList.Count -1; i++)
            {
                ItemInfoList[i].DisposeUnmanagedData();
            }
        }

        public void ClearCache()
        {          
            ItemInfoList.Clear();

            if(RenderInstancesList == null)
            {
                return;
            }

            for (int j = 0; j <= RenderInstancesList.ItemComputeBufferList.Count - 1; j++)
            {
                if (RenderInstancesList.ItemComputeBufferList[j].Created)
                {
                    RenderInstancesList.ItemComputeBufferList[j].ComputeBuffer.Dispose();
                    RenderInstancesList.ItemComputeBufferList[j].Created = false;
                }
            }

            ClearInstanceMemory();
        }

        public void RemoveItemInstances(int ID)
        {
            ItemInfo persistentInfo = GetPersistentInfo(ID);
            if (persistentInfo != null)
            {
                persistentInfo.InstanceDataList.Clear();
                persistentInfo.InstanceDataList.Capacity = 0;
            }
        }

        public void ConvertCellPersistentStorageToRenderData(QuadroRenderer quadroRenderer)
        {
            if(RenderInstancesList == null)
            {
                RenderInstancesList = new RenderInstances(quadroRenderer.QuadroPrototypesPackage.PrototypeList.Count);
            }

            for (int protoIndex = 0; protoIndex <= quadroRenderer.QuadroPrototypesPackage.PrototypeList.Count - 1; protoIndex++)
            {
                QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.PrototypeList[protoIndex];

                if(proto == null)
                { 
                    continue;
                }

                UpdatePersistentStorageToMatrix(proto, protoIndex);
                UpdateRenderBuffer(protoIndex);
            }
        }

        public void UpdatePersistentStorageToMatrix(QuadroPrototype proto, int itemIndex)
        {
            ItemInfo persistentInfo = GetPersistentInfo(proto.ID);
 
            if(persistentInfo == null)
            {
                return;
            }

            NativeList<Matrix4x4> matrixList = RenderInstancesList.ItemMatrixList[itemIndex];
            matrixList.Clear();     

            if(persistentInfo.InstanceDataList.Count != 0)
            {
                persistentInfo.CopyToNativeArray();

                matrixList.ResizeUninitialized(persistentInfo.NativeInstanceDataArray.Length); 

                LoadPersistentStorageToMatrixWideJob loadPersistentStorageToMatrixJob =
                new LoadPersistentStorageToMatrixWideJob
                {
                    InstanceList = persistentInfo.NativeInstanceDataArray,
                    MatrixList = matrixList.AsDeferredJobArray(),
                };

                loadPersistentStorageToMatrixJob.Schedule(matrixList, 64).Complete();
            }
        }

        public void UpdateRenderBuffer(int itemIndex)
        {
            NativeArray<Matrix4x4> indirectInstanceInfo = RenderInstancesList.ItemMatrixList[itemIndex];
            ComputeBufferInfo computeBufferInfo = RenderInstancesList.ItemComputeBufferList[itemIndex];

            if(indirectInstanceInfo.Length == 0)
            {
                if(computeBufferInfo.ComputeBuffer != null)
                {
                    computeBufferInfo.ComputeBuffer.Release();
                    computeBufferInfo.ComputeBuffer = null; 
                    computeBufferInfo.Created = false;
                }
            }
            else
            {
                int length = indirectInstanceInfo.Length;                         
                if (length == 0) length = 1;

                if(computeBufferInfo.ComputeBuffer != null)
                {
                    computeBufferInfo.ComputeBuffer.Release();
                }

                computeBufferInfo.ComputeBuffer = new ComputeBuffer(length, QuadroRendererConstants.STRIDE_SIZE_MATRIX4X4);
                computeBufferInfo.ComputeBuffer.SetData(indirectInstanceInfo);
                computeBufferInfo.Created = true;
            }
        }

        public void ConvertPersistentItemToObjectInstanced()
        {
            foreach (ItemInfo info in ItemInfoList)
            {
                info.ObjectInstanceData.Clear();
                
                foreach (InstanceData item in info.InstanceDataList)
                {
                    info.ObjectInstanceData.Add(new ObjectInstanceData(item));
                }
            }
        }

        public void ConvertObjectInstancedToPersistentItem()
        {
            foreach (ItemInfo info in ItemInfoList)
            {
                if(info.ObjectInstanceData.Count != 0)
                {
                    info.InstanceDataList.Clear();
                
                    foreach (ObjectInstanceData instanced in info.ObjectInstanceData)
                    {
                        InstanceData persistentItem = new InstanceData
                        {
                            Position = instanced.Position,
                            Rotation = instanced.Rotation,
                            Scale = instanced.Scale,
                        };

                        info.AddPersistentItemInstance(ref persistentItem);
                    }
                }
            }
        }

        public void ClearObjectInstancedList()
        {
            foreach (ItemInfo info in ItemInfoList)
            {
                info.ObjectInstanceData.Clear();
            }
        }
    }
}
