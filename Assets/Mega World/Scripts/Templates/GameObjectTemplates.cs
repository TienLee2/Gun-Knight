#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MegaWorld
{
    public static class GameObjectTemplates 
    {
    	public static void ApplyBigTreesTemplate(Type typeOfMegaWorld, PrototypeGameObject proto)
    	{
            TransformComponentsStack transformComponentsStack = proto.TransformComponentsStack;
            FilterStack  maskFilterStack = proto.MaskFilterStack;
            OverlapCheckSettings overlapCheckSettings = proto.OverlapCheckSettings;
            FailureSettings failureSettings = proto.FailureSettings;

    		typeOfMegaWorld.ScatterSettings.Grid.RandomisationType = RandomisationType.Square;
    		typeOfMegaWorld.ScatterSettings.Grid.Vastness = 1;
    		typeOfMegaWorld.ScatterSettings.Grid.GridStep = new Vector2(3, 3);

    		#region Transform Components
    		transformComponentsStack.TransformComponents.Clear();

    		TreeRotation treeRotation = ScriptableObject.CreateInstance<TreeRotation>();
            treeRotation.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            treeRotation.name = "TreeRotation";

    		Align align = ScriptableObject.CreateInstance<Align>();
            align.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            align.name = "Align";

    		PositionOffset positionOffset = ScriptableObject.CreateInstance<PositionOffset>();
            positionOffset.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            positionOffset.name = "PositionOffset";

    		SlopePosition slopePosition = ScriptableObject.CreateInstance<SlopePosition>();
            slopePosition.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            slopePosition.name = "SlopePosition";

    		Scale scale = ScriptableObject.CreateInstance<Scale>();
            scale.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scale.name = "Scale";

    		ScaleFitness scaleFitness = ScriptableObject.CreateInstance<ScaleFitness>();
            scaleFitness.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scaleFitness.name = "ScaleFitness";

    		ScaleClamp scaleClamp = ScriptableObject.CreateInstance<ScaleClamp>();
            scaleClamp.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scaleClamp.name = "ScaleClamp";

    		transformComponentsStack.TransformComponents.Add(treeRotation);
    		transformComponentsStack.TransformComponents.Add(align);
    		transformComponentsStack.TransformComponents.Add(positionOffset);
    		transformComponentsStack.TransformComponents.Add(slopePosition);
    		transformComponentsStack.TransformComponents.Add(scale);
    		transformComponentsStack.TransformComponents.Add(scaleFitness);
    		transformComponentsStack.TransformComponents.Add(scaleClamp);

    		AssetDatabase.AddObjectToAsset(treeRotation, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(align, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(positionOffset, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(slopePosition, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(scale, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(scaleFitness, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(scaleClamp, typeOfMegaWorld); 
    		#endregion

    		#region Mask Filters
    		maskFilterStack.Filters.Clear();

    		HeightFilter heightFilter = ScriptableObject.CreateInstance<HeightFilter>();
            heightFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            heightFilter.name = "HeightFilter";
    		heightFilter.MinHeight = 0;
    		heightFilter.MaxHeight = 620;
    		heightFilter.AddHeightFalloff = 100;

    		SlopeFilter slopeFilter = ScriptableObject.CreateInstance<SlopeFilter>();
            slopeFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            slopeFilter.name = "SlopeFilter";

    		maskFilterStack.Filters.Add(heightFilter);
    		maskFilterStack.Filters.Add(slopeFilter);

    		AssetDatabase.AddObjectToAsset(heightFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(slopeFilter, typeOfMegaWorld);
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShape = OverlapShape.Sphere;
    		overlapCheckSettings.SphereCheck.vegetationMode = true;
    		overlapCheckSettings.SphereCheck.priority = 0;
    		overlapCheckSettings.SphereCheck.viabilitySize = 4f;
    		overlapCheckSettings.SphereCheck.trunkSize = 0.8f;
    		#endregion

    		#region FailureSettings
    		failureSettings.Enable = true;
    		failureSettings.FailureRate = 80f;
    		failureSettings.FailureRateFromFitness.keys.Clear();
    		failureSettings.FailureRateFromFitness.AddKey(Color.black, 0.8f);
    		failureSettings.FailureRateFromFitness.AddKey(Color.white, 1f);
    		#endregion

            AssetDatabase.SaveAssets();
    	}

    	public static void ApplySmallTreesTemplate(Type typeOfMegaWorld, PrototypeGameObject proto)
    	{
            TransformComponentsStack transformComponentsStack = proto.TransformComponentsStack;
            FilterStack  maskFilterStack = proto.MaskFilterStack;
            OverlapCheckSettings overlapCheckSettings = proto.OverlapCheckSettings;
            FailureSettings failureSettings = proto.FailureSettings;

    		typeOfMegaWorld.ScatterSettings.Grid.RandomisationType = RandomisationType.Square;
    		typeOfMegaWorld.ScatterSettings.Grid.Vastness = 1;
    		typeOfMegaWorld.ScatterSettings.Grid.GridStep = new Vector2(5, 5);

    		#region Transform Components
    		transformComponentsStack.TransformComponents.Clear();

    		TreeRotation treeRotation = ScriptableObject.CreateInstance<TreeRotation>();
            treeRotation.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            treeRotation.name = "TreeRotation";

    		Align align = ScriptableObject.CreateInstance<Align>();
            align.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            align.name = "Align";

    		PositionOffset positionOffset = ScriptableObject.CreateInstance<PositionOffset>();
            positionOffset.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            positionOffset.name = "PositionOffset";

    		SlopePosition slopePosition = ScriptableObject.CreateInstance<SlopePosition>();
            slopePosition.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            slopePosition.name = "SlopePosition";

    		Scale scale = ScriptableObject.CreateInstance<Scale>();
            scale.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scale.name = "Scale";

    		transformComponentsStack.TransformComponents.Add(treeRotation);
    		transformComponentsStack.TransformComponents.Add(align);
    		transformComponentsStack.TransformComponents.Add(positionOffset);
    		transformComponentsStack.TransformComponents.Add(slopePosition);
    		transformComponentsStack.TransformComponents.Add(scale);

    		AssetDatabase.AddObjectToAsset(treeRotation, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(align, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(positionOffset, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(slopePosition, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(scale, typeOfMegaWorld);  
    		#endregion

    		#region Mask Filters
    		maskFilterStack.Filters.Clear();

    		HeightFilter heightFilter = ScriptableObject.CreateInstance<HeightFilter>();
            heightFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            heightFilter.name = "HeightFilter";
    		heightFilter.MinHeight = 0;
    		heightFilter.MaxHeight = 620;
    		heightFilter.AddHeightFalloff = 100;

    		SlopeFilter slopeFilter = ScriptableObject.CreateInstance<SlopeFilter>();
            slopeFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            slopeFilter.name = "SlopeFilter";

    		maskFilterStack.Filters.Add(heightFilter);
    		maskFilterStack.Filters.Add(slopeFilter);

    		AssetDatabase.AddObjectToAsset(heightFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(slopeFilter, typeOfMegaWorld);
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShape = OverlapShape.Sphere;
    		overlapCheckSettings.SphereCheck.vegetationMode = true;
    		overlapCheckSettings.SphereCheck.priority = 1;
    		overlapCheckSettings.SphereCheck.trunkSize = 0.4f;
    		overlapCheckSettings.SphereCheck.viabilitySize = 2f;
    		#endregion

    		#region FailureSettings
    		failureSettings.Enable = true;
    		failureSettings.FailureRate = 65f;
    		failureSettings.FailureRateFromFitness.keys.Clear();
    		failureSettings.FailureRateFromFitness.AddKey(Color.black, 0f);
    		failureSettings.FailureRateFromFitness.AddKey(Color.white, 1f);
    		#endregion

            AssetDatabase.SaveAssets();
    	}

    	public static void ApplyBigCliffsTemplate(Type typeOfMegaWorld, PrototypeGameObject proto)
    	{
            TransformComponentsStack transformComponentsStack = proto.TransformComponentsStack;
            FilterStack  maskFilterStack = proto.MaskFilterStack;
            OverlapCheckSettings overlapCheckSettings = proto.OverlapCheckSettings;
            FailureSettings failureSettings = proto.FailureSettings;

    		typeOfMegaWorld.ScatterSettings.Grid.RandomisationType = RandomisationType.Square;
    		typeOfMegaWorld.ScatterSettings.Grid.Vastness = 1;
    		typeOfMegaWorld.ScatterSettings.Grid.GridStep = new Vector2(1.7f, 1.7f);

    		#region Transform Components
    		transformComponentsStack.TransformComponents.Clear();

    		CliffsAlign cliffsAlign = ScriptableObject.CreateInstance<CliffsAlign>();
            cliffsAlign.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            cliffsAlign.name = "CliffsAlign";

    		SlopePosition slopePosition = ScriptableObject.CreateInstance<SlopePosition>();
            slopePosition.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            slopePosition.name = "SlopePosition";

    		Scale scale = ScriptableObject.CreateInstance<Scale>();
            scale.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scale.name = "Scale";
    		scale.maxScale = new Vector3(1.4f, 1.4f, 1.4f);

    		ScaleFitness scaleFitness = ScriptableObject.CreateInstance<ScaleFitness>();
            scaleFitness.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scaleFitness.name = "ScaleFitness";
    		scaleFitness.OffsetUniformScale = -1;

    		transformComponentsStack.TransformComponents.Add(cliffsAlign);
    		transformComponentsStack.TransformComponents.Add(slopePosition);
    		transformComponentsStack.TransformComponents.Add(scale);
    		transformComponentsStack.TransformComponents.Add(scaleFitness);

    		AssetDatabase.AddObjectToAsset(cliffsAlign, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(slopePosition, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(scale, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(scaleFitness, typeOfMegaWorld); 
    		#endregion

    		#region Mask Filters
    		maskFilterStack.Filters.Clear();

    		NoiseFilter noiseFilter = ScriptableObject.CreateInstance<NoiseFilter>();
            noiseFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            noiseFilter.name = "NoiseFilter";
            noiseFilter.NoiseSettings = new NoiseSettings();
    		noiseFilter.NoiseSettings.TransformSettings = new NoiseSettings.NoiseTransformSettings();
    		noiseFilter.NoiseSettings.TransformSettings.Scale = new Vector3(31, 40, 31);

    		RemapFilter remapFilter = ScriptableObject.CreateInstance<RemapFilter>();
            remapFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            remapFilter.name = "RemapFilter";
    		remapFilter.Min = 0.44f;
    		remapFilter.Max = 0.47f;

    		SlopeFilter slopeFilter = ScriptableObject.CreateInstance<SlopeFilter>();
            slopeFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            slopeFilter.name = "SlopeFilter";
    		slopeFilter.MinSlope = 48;
    		slopeFilter.MaxSlope = 90;
    		slopeFilter.AddSlopeFalloff = 17;

    		maskFilterStack.Filters.Add(noiseFilter);
    		maskFilterStack.Filters.Add(remapFilter);
    		maskFilterStack.Filters.Add(slopeFilter);

    		AssetDatabase.AddObjectToAsset(noiseFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(remapFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(slopeFilter, typeOfMegaWorld);
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShape = OverlapShape.None;
    		#endregion

    		#region FailureSettings
    		failureSettings.Enable = true;
    		failureSettings.FailureRate = 60f;
    		failureSettings.FailureRateFromFitness.keys.Clear();
    		failureSettings.FailureRateFromFitness.AddKey(Color.black, 0f);
    		failureSettings.FailureRateFromFitness.AddKey(Color.white, 1f);
    		#endregion

            AssetDatabase.SaveAssets();
    	}

    	public static void ApplySmallCliffsTemplate(Type typeOfMegaWorld, PrototypeGameObject proto)
    	{
            TransformComponentsStack transformComponentsStack = proto.TransformComponentsStack;
            FilterStack  maskFilterStack = proto.MaskFilterStack;
            OverlapCheckSettings overlapCheckSettings = proto.OverlapCheckSettings;
            FailureSettings failureSettings = proto.FailureSettings;
            
    		typeOfMegaWorld.ScatterSettings.Grid.RandomisationType = RandomisationType.Square;
    		typeOfMegaWorld.ScatterSettings.Grid.Vastness = 1;
    		typeOfMegaWorld.ScatterSettings.Grid.GridStep = new Vector2(1.3f, 1.3f);

    		#region Transform Components
    		transformComponentsStack.TransformComponents.Clear();

    		Rotation rotation = ScriptableObject.CreateInstance<Rotation>();
            rotation.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            rotation.name = "Rotation";
    		rotation.RandomizeOrientationX = 100;
    		rotation.RandomizeOrientationY = 100;
    		rotation.RandomizeOrientationZ = 100;

    		Scale scale = ScriptableObject.CreateInstance<Scale>();
            scale.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scale.name = "Scale";
    		scale.minScale = new Vector3(0.8f, 0.8f, 0.8f);
    		scale.maxScale = new Vector3(1.2f, 1.2f, 1.2f);

    		transformComponentsStack.TransformComponents.Add(rotation);
    		transformComponentsStack.TransformComponents.Add(scale);

    		AssetDatabase.AddObjectToAsset(rotation, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(scale, typeOfMegaWorld);  
    		#endregion

    		#region Mask Filters
    		maskFilterStack.Filters.Clear();

    		NoiseFilter noiseFilter = ScriptableObject.CreateInstance<NoiseFilter>();
            noiseFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            noiseFilter.name = "NoiseFilter";
            noiseFilter.NoiseSettings = new NoiseSettings();
    		noiseFilter.NoiseSettings.TransformSettings = new NoiseSettings.NoiseTransformSettings();
    		noiseFilter.NoiseSettings.TransformSettings.Scale = new Vector3(31, 40, 31);

    		InvertFilter invertFilter = ScriptableObject.CreateInstance<InvertFilter>();
            invertFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            invertFilter.name = "InvertFilter";
    		invertFilter.StrengthInvert = 1;

    		RemapFilter remapFilter = ScriptableObject.CreateInstance<RemapFilter>();
            remapFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            remapFilter.name = "RemapFilter";
    		remapFilter.Min = 0.44f;
    		remapFilter.Max = 0.47f;

    		SlopeFilter slopeFilter = ScriptableObject.CreateInstance<SlopeFilter>();
            slopeFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            slopeFilter.name = "SlopeFilter";
    		slopeFilter.MinSlope = 48;
    		slopeFilter.MaxSlope = 90;
    		slopeFilter.AddSlopeFalloff = 28;

    		maskFilterStack.Filters.Add(noiseFilter);
    		maskFilterStack.Filters.Add(invertFilter);
    		maskFilterStack.Filters.Add(remapFilter);
    		maskFilterStack.Filters.Add(slopeFilter);

    		AssetDatabase.AddObjectToAsset(noiseFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(invertFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(remapFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(slopeFilter, typeOfMegaWorld);
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShape = OverlapShape.None;
    		#endregion

    		#region FailureSettings
    		failureSettings.Enable = true;
    		failureSettings.FailureRate = 60f;
    		failureSettings.FailureRateFromFitness.keys.Clear();
    		failureSettings.FailureRateFromFitness.AddKey(Color.black, 0f);
    		failureSettings.FailureRateFromFitness.AddKey(Color.white, 1f);
    		#endregion

            AssetDatabase.SaveAssets();
    	}

    	public static void ApplyRocksTemplate(Type typeOfMegaWorld, PrototypeGameObject proto)
    	{
            TransformComponentsStack transformComponentsStack = proto.TransformComponentsStack;
            FilterStack  maskFilterStack = proto.MaskFilterStack;
            OverlapCheckSettings overlapCheckSettings = proto.OverlapCheckSettings;
            FailureSettings failureSettings = proto.FailureSettings;

    		typeOfMegaWorld.ScatterSettings.Grid.RandomisationType = RandomisationType.Square;
    		typeOfMegaWorld.ScatterSettings.Grid.Vastness = 1;
    		typeOfMegaWorld.ScatterSettings.Grid.GridStep = new Vector2(2.7f, 2.7f);

    		#region Transform Components
    		transformComponentsStack.TransformComponents.Clear();

    		Rotation rotation = ScriptableObject.CreateInstance<Rotation>();
            rotation.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            rotation.name = "Rotation";
    		rotation.RandomizeOrientationX = 100;
    		rotation.RandomizeOrientationY = 100;
    		rotation.RandomizeOrientationZ = 100;

    		Scale scale = ScriptableObject.CreateInstance<Scale>();
            scale.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            scale.name = "Scale";
    		scale.minScale = new Vector3(0.8f, 0.8f, 0.8f);
    		scale.maxScale = new Vector3(1.2f, 1.2f, 1.2f);

    		transformComponentsStack.TransformComponents.Add(rotation);
    		transformComponentsStack.TransformComponents.Add(scale);

    		AssetDatabase.AddObjectToAsset(rotation, typeOfMegaWorld); 
    		AssetDatabase.AddObjectToAsset(scale, typeOfMegaWorld);  
    		#endregion

    		#region Mask Filters
    		maskFilterStack.Filters.Clear();

    		NoiseFilter noiseFilter = ScriptableObject.CreateInstance<NoiseFilter>();
            noiseFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            noiseFilter.name = "NoiseFilter";
            noiseFilter.NoiseSettings = new NoiseSettings();
    		noiseFilter.NoiseSettings.TransformSettings = new NoiseSettings.NoiseTransformSettings();
    		noiseFilter.NoiseSettings.TransformSettings.Scale = new Vector3(37, 40, 37);

    		RemapFilter remapFilter = ScriptableObject.CreateInstance<RemapFilter>();
            remapFilter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            remapFilter.name = "RemapFilter";
    		remapFilter.Min = 0.41f;
    		remapFilter.Max = 0.5f;

    		maskFilterStack.Filters.Add(noiseFilter);
    		maskFilterStack.Filters.Add(remapFilter);

    		AssetDatabase.AddObjectToAsset(noiseFilter, typeOfMegaWorld);  
    		AssetDatabase.AddObjectToAsset(remapFilter, typeOfMegaWorld);  
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShape = OverlapShape.Bounds;
    		overlapCheckSettings.BoundsCheck.boundsType = BoundsCheckType.BoundsPrefab;
    		overlapCheckSettings.BoundsCheck.multiplyBoundsSize = 1;
    		#endregion

    		#region FailureSettings
    		failureSettings.Enable = true;
    		failureSettings.FailureRate = 90f;
    		failureSettings.FailureRateFromFitness.keys.Clear();
    		failureSettings.FailureRateFromFitness.AddKey(Color.black, 0f);
    		failureSettings.FailureRateFromFitness.AddKey(Color.white, 1f);
    		#endregion

            AssetDatabase.SaveAssets();
    	}
    }
}
#endif