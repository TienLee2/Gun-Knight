using UnityEngine;

namespace MegaWorld
{
    public class InstanceData
    {
        public Vector3 position;
        public Vector3 scale; 
        public Quaternion rotation;
        public float fitness;

        public InstanceData()
        {
            
        }
        
        public InstanceData(Vector3 position, Vector3 scaleFactor, Quaternion rotation, float fitness)
        {
            this.position = position;
            this.scale = scaleFactor; 
            this.rotation = rotation;
            this.fitness = fitness;
        }
    }
}

