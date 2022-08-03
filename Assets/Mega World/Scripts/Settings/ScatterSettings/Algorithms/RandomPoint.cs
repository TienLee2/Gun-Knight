using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class RandomPoint 
    {
        public int NumberOfChecks = 30;
        public bool OnlyOneCheck = false;
        public bool MinMaxSlider = true;
        public int Instance = 15;
        public int MinChecks = 15;
		public int MaxChecks = 15;

        public void SetPositionForStamper(Type type, PainterVariables painterVariables, Func<Vector3, bool> func)
        {
            Vector3 positionForSpawn;

            for (int checks = 0; checks < NumberOfChecks; checks++)
            {
                positionForSpawn = GetRandomSquareForStamper(type, painterVariables, painterVariables.RaycastInfo);

                func.Invoke(positionForSpawn);
            }
        }

        public void SetPosition(Type type, PainterVariables painterVariables, Func<Vector3, bool> func)
        {
            Vector3 positionForSpawn;

            long numberOfChecks = 0;
    
            if(OnlyOneCheck == true)
            {
                numberOfChecks = 1;
            }
            else
            {
                numberOfChecks = MinMaxSlider == true ? UnityEngine.Random.Range((int)MinChecks, (int)MaxChecks) : Instance;
            }

            if(numberOfChecks == 1)
            {
                positionForSpawn = painterVariables.RaycastInfo.hitInfo.point + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, painterVariables.RaycastInfo.hitInfo.normal) * painterVariables.Radius;

                func.Invoke(positionForSpawn);
            }
            else
            {
                for (int checks = 0; checks < numberOfChecks; checks++)
                {
                    positionForSpawn = GetRandomSquareFromBrushTool(type, painterVariables, painterVariables.RaycastInfo);

                    func.Invoke(positionForSpawn);
                }
            }
        }

        public Vector3 GetRandomSquareForStamper(Type type, PainterVariables painterVariables, RaycastInfo originalRaycastInfo)
        {
            Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-painterVariables.Radius, painterVariables.Radius), 0f, UnityEngine.Random.Range(-painterVariables.Radius, painterVariables.Radius));
            return originalRaycastInfo.hitInfo.point + spawnOffset;
        }

        public Vector3 GetRandomSquareFromBrushTool(Type type, PainterVariables painterVariables, RaycastInfo originalRaycastInfo)
        {
            if(type.ResourceType == ResourceType.GameObject)
            {
                Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                return originalRaycastInfo.hitInfo.point + Vector3.ProjectOnPlane(spawnOffset, originalRaycastInfo.hitInfo.normal) * painterVariables.Radius;
            }
            else
            {
                Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-painterVariables.Radius, painterVariables.Radius), 0f, UnityEngine.Random.Range(-painterVariables.Radius, painterVariables.Radius));
                return originalRaycastInfo.hitInfo.point + spawnOffset;
            }
        }
    }
}