using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu()]
public class SimulationSettings : ScriptableObject
{
    [Header("Simulation Settings")]
    public int numAgents = 100;
    public int vpWidth = 256;
    public int vpHeight = 256;
    public int simsPerFrame = 10;
    public float decayRate = 1;
    public float diffuseRate = 1;
    public int foodBrushRadius = 5;
    public Vector4 foodColor = new Vector4(0.882f, 0.682f, 0.376f, 1.0f);

    [System.Serializable]
    public struct SpeciesSettings
    {
        public float sensorAngle;// 20 * (Mathf.PI / 180);
        public float rotationAngle; //45 * (Mathf.PI / 180); 
        // should be bigger than sensorAngle to avoid convergence, adjusted by random offset (ranging 0, 45)? in compute shader   
        public int sensorDist;
        public int sensorRadius;
        public float velocity;
        public float trailWeight;
        public Vector4 color;
    };

    public SpeciesSettings[] species;
}