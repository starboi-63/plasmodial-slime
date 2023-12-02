using UnityEngine;

[CreateAssetMenu()]
public class SimulationSettings : ScriptableObject
{
    [Header("Simulation Settings")]
    public int numAgents = 100;
    public int windowWidth = 1920;
    public int windowHeight = 1080;
}