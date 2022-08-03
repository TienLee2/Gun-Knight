using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace QuadroRendererSystem
{
    public class ItemLODMatrix
    {
        [NonSerialized]
        public readonly List<NativeList<Matrix4x4>> ItemMatrixList = new List<NativeList<Matrix4x4>>();
    }

    public class ItemLODLodFade
    {
        [NonSerialized]
        public readonly List<NativeList<Vector4>> ItemLodFadeList = new List<NativeList<Vector4>>();
    }

    public class CameraRenderData
    {
        public int LODCount;

        [NonSerialized]
        public readonly List<NativeList<Matrix4x4>> ItemMergeMatrixList = new List<NativeList<Matrix4x4>>();

        public List<ItemLODMatrix> ItemLODMatrixList = new List<ItemLODMatrix>();
        public List<ItemLODMatrix> ItemLODShadowMatrixList = new List<ItemLODMatrix>();
        public List<ItemLODLodFade> ItemLODFadeList = new List<ItemLODLodFade>();

        public CameraRenderData(int itemCount)
        {
            this.LODCount = QuadroRendererConstants.QuadroRendererSettings.MaxLODCountSupport;

            for (int LODIndex = 0; LODIndex < LODCount; LODIndex++)
            {
                ItemLODMatrixList.Add(new ItemLODMatrix());
                ItemLODShadowMatrixList.Add(new ItemLODMatrix());
                ItemLODFadeList.Add(new ItemLODLodFade());
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                NativeList<Matrix4x4> newMatrixList = new NativeList<Matrix4x4>(1024, Allocator.Persistent) { Capacity = 1024 };
                ItemMergeMatrixList.Add(newMatrixList);

                for (int LODIndex = 0; LODIndex < LODCount; LODIndex++)
                {
                    NativeList<Matrix4x4> newMatrixLOD0List = new NativeList<Matrix4x4>(1024, Allocator.Persistent) { Capacity = 1024 };
                    ItemLODMatrixList[LODIndex].ItemMatrixList.Add(newMatrixLOD0List);

                    NativeList<Matrix4x4> newMatrixLOD0ShadowList = new NativeList<Matrix4x4>(1024, Allocator.Persistent) { Capacity = 1024 };
                    ItemLODShadowMatrixList[LODIndex].ItemMatrixList.Add(newMatrixLOD0ShadowList);

                    NativeList<Vector4> newLOD0LodFadeList = new NativeList<Vector4>(1024, Allocator.Persistent) { Capacity = 1024 };
                    ItemLODFadeList[LODIndex].ItemLodFadeList.Add(newLOD0LodFadeList);
                }
            }
        }

        public void ClearRenderData(int protoIndex)
        {
            ItemMergeMatrixList[protoIndex].Clear();

            for (int LODIndex = 0; LODIndex < LODCount; LODIndex++)
            {
                ItemLODMatrixList[LODIndex].ItemMatrixList[protoIndex].Clear();
                ItemLODShadowMatrixList[LODIndex].ItemMatrixList[protoIndex].Clear();
                ItemLODFadeList[LODIndex].ItemLodFadeList[protoIndex].Clear();
            }
        }

        public void DisposeData()
        {
            DisposeMatrixInstanceList(ItemMergeMatrixList);

            for (int LODIndex = 0; LODIndex < LODCount; LODIndex++)
            {
                DisposeMatrixList(ItemLODMatrixList[LODIndex].ItemMatrixList);
                DisposeMatrixList(ItemLODShadowMatrixList[LODIndex].ItemMatrixList);
                DisposeVector4List(ItemLODFadeList[LODIndex].ItemLodFadeList);
            }
        }

        void DisposeMatrixList(List<NativeList<Matrix4x4>> list)
        {
            for (int i = 0; i <= list.Count - 1; i++)
            {
                list[i].Dispose();
            }
        }

        void DisposeMatrixInstanceList(List<NativeList<Matrix4x4>> list)
        {
            for (int i = 0; i <= list.Count - 1; i++)
            {
                list[i].Dispose();
            }
        }

        void DisposeVector4List(List<NativeList<Vector4>> list)
        {
            for (int i = 0; i <= list.Count - 1; i++)
            {
                list[i].Dispose();
            }
        }
    }
}