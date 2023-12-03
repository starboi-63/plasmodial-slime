using UnityEngine;

[CreateAssetMenu()]
public class SimulationSettings : ScriptableObject
{
    [Header("Simulation Settings")]
    public int numAgents = 1;
    public int vpWidth = 256;
    public int vpHeight = 256;
    public int simsPerFrame = 10;
}