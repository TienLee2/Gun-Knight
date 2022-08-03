#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class OverlapCheckSettingsEditor 
    {
        public bool overlapСheckFoldout = true;

        public void OnGUI(OverlapCheckSettings overlapCheckSettings)
        {
            OverlapCheckSettingsWindowGUI(overlapCheckSettings);
        }

        public void OverlapCheckSettingsWindowGUI(OverlapCheckSettings overlapCheckSettings)
		{
			overlapСheckFoldout = CustomEditorGUI.Foldout(overlapСheckFoldout, "Overlap Check Settings");

			if(overlapСheckFoldout)
			{
				EditorGUI.indentLevel++;
				
				overlapCheckSettings.OverlapShape = (OverlapShape)CustomEditorGUI.EnumPopup(overlapShape, overlapCheckSettings.OverlapShape);

				EditorGUI.indentLevel++;

				switch (overlapCheckSettings.OverlapShape)
				{
					case OverlapShape.Bounds:
					{
						BoundsCheck boundsCheck = overlapCheckSettings.BoundsCheck;

						boundsCheck.boundsType = (BoundsCheckType)CustomEditorGUI.EnumPopup(boundsType, boundsCheck.boundsType);

						if(boundsCheck.boundsType == BoundsCheckType.Custom)
						{
							boundsCheck.uniformBoundsSize = CustomEditorGUI.Toggle(uniformBoundsSize, boundsCheck.uniformBoundsSize);

							if(boundsCheck.uniformBoundsSize)
							{
								boundsCheck.boundsSize.x = CustomEditorGUI.FloatField(boundsSize, boundsCheck.boundsSize.x);

								boundsCheck.boundsSize.z = boundsCheck.boundsSize.x;
								boundsCheck.boundsSize.y = boundsCheck.boundsSize.x;
							}
							else
							{
								boundsCheck.boundsSize = CustomEditorGUI.Vector3Field(boundsSize, boundsCheck.boundsSize);
							}

							boundsCheck.multiplyBoundsSize = CustomEditorGUI.Slider(multiplyBoundsSize, boundsCheck.multiplyBoundsSize, 0, 5);
						}
						else if(boundsCheck.boundsType == BoundsCheckType.BoundsPrefab)
						{
							boundsCheck.multiplyBoundsSize = CustomEditorGUI.Slider(multiplyBoundsSize, boundsCheck.multiplyBoundsSize, 0, 5);
						}
						break;
					}
					case OverlapShape.Sphere:
					{
						SphereCheck sphereCheck = overlapCheckSettings.SphereCheck;

						sphereCheck.vegetationMode = CustomEditorGUI.Toggle(vegetationMode, sphereCheck.vegetationMode);

						if(sphereCheck.vegetationMode)
						{
							sphereCheck.priority = CustomEditorGUI.IntField(priority, sphereCheck.priority);
							sphereCheck.trunkSize = CustomEditorGUI.Slider(trunkSize, sphereCheck.trunkSize, 0, 10);
							sphereCheck.viabilitySize = CustomEditorGUI.FloatField(viabilitySize, sphereCheck.viabilitySize);

							if(sphereCheck.viabilitySize < overlapCheckSettings.SphereCheck.trunkSize)
							{
								sphereCheck.viabilitySize = overlapCheckSettings.SphereCheck.trunkSize;
							}
						}
						else
						{
							sphereCheck.size = CustomEditorGUI.FloatField(size, sphereCheck.size);
						}
						break;
					}
				}

				EditorGUI.indentLevel--;

				CollisionCheck collisionCheck = overlapCheckSettings.CollisionCheck;

				collisionCheck.collisionCheckType =  CustomEditorGUI.Toggle(new GUIContent("Collision Check"), collisionCheck.collisionCheckType);
				
				if(collisionCheck.collisionCheckType)
				{
					EditorGUI.indentLevel++;

					collisionCheck.multiplyBoundsSize = CustomEditorGUI.Slider(multiplyBoundsSize, collisionCheck.multiplyBoundsSize, 0, 10);
					collisionCheck.checkCollisionLayers = CustomEditorGUI.LayerField(new GUIContent("Check Collision Layers"), collisionCheck.checkCollisionLayers);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}
		}

		[NonSerialized]
		public GUIContent overlapShape = new GUIContent("Overlap Shape", "What shape will be checked for intersection with other prototypes. Overlap Shape only works with added prototypes in MegaWorld. Overlap Chap can be Bounds and Sphere.");

		#region Bounds Check
		[NonSerialized]
		public GUIContent boundsType = new GUIContent("Bounds Type", "Which Bounds will be used.");
		[NonSerialized]
		public GUIContent uniformBoundsSize = new GUIContent("Uniform Bounds Size", "Each side of the Bounds has the same size value.");
		[NonSerialized]
		public GUIContent boundsSize = new GUIContent("Bounds Size", "Lets you choose the size of the vector for bounds size.");
		[NonSerialized]
		public GUIContent multiplyBoundsSize = new GUIContent("Multiply Bounds Size", "Allows you to resize the bounds.");
		#endregion

		#region Sphere Variables
		[NonSerialized]
		public GUIContent vegetationMode = new GUIContent("Vegetation Mode", "Allows you to use the priority system, which allows for example small trees to spawn under a large tree.");
		[NonSerialized]
		public GUIContent priority = new GUIContent("Priority", "Sets the ability of the object so that the object can spawn around the Viability Size of another object whose this value is less.");
		[NonSerialized]
		public GUIContent trunkSize = new GUIContent("Trunk Size", "Sets the size of the trunk. Other objects will never be spawn in this size.");
		[NonSerialized]
		public GUIContent viabilitySize = new GUIContent("Viability Size", " This is size in which other objects will not be spawned if Priority is less.");
		[NonSerialized]
		public GUIContent size = new GUIContent("Size", "The size of the sphere that will not spawn.");
		#endregion

		#region Collision Check
		[NonSerialized]
		public GUIContent collisionCheckType = new GUIContent("Collision Check", "Used to prevent the object from spawning inside the GameObject. This is a useful feature to prevent an object from spawning inside a building, for example. Overlap Shape only works with added prototypes in MegaWorld, and this function allows you to check for overlaps for all GameObjects.");
		[NonSerialized]
		public GUIContent checkCollisionLayers = new GUIContent("Check Collision Layers", "Layer to be checked for overlap.");
		#endregion
    }
}
#endif
