using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ComputeUtilities;
using Unity.VisualScripting;

public class SlimeSimulation : MonoBehaviour
{
    public RawImage viewport;

    public SimulationSettings settings;

    public ComputeShader computeSim;

    const int updateKernel = 0;
    const int blurKernel = 1;
	const int paintKernel = 2;
    const int clearKernel = 3;

    public RenderTexture viewportTex;
    public RenderTexture trailMap;
    public RenderTexture nextTrailMap;

    ComputeBuffer agentBuffer;
    ComputeBuffer speciesBuffer;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize agent, trail, and color buffers
        ComputeUtil.CreateTex(ref viewportTex, settings.vpWidth, settings.vpHeight);
        ComputeUtil.CreateTex(ref trailMap, settings.vpWidth, settings.vpHeight);
        ComputeUtil.CreateTex(ref nextTrailMap, settings.vpWidth, settings.vpHeight);

        viewport.texture = viewportTex;

        computeSim.SetTexture(updateKernel, "TrailMap", trailMap);

        computeSim.SetTexture(blurKernel, "TrailMap",  trailMap);
        computeSim.SetTexture(blurKernel, "NextTrailMap", nextTrailMap);
    

        computeSim
        
        // clearing trail and viewport textures (setting to 0)
        computeSim.SetTexture(clearKernel, "TrailMap",  trailMap);
        computeSim.Dispatch(clearKernel, settings.vpWidth / 8, settings.vpHeight / 8, 1);

        SlimeAgent[] agents = new SlimeAgent[settings.numAgents];
        for (int i = 0; i < settings.numAgents; i++) {
            float randAngle = Random.value * 2 * Mathf.PI;
            agents[i] = new SlimeAgent {
                position = new Vector2(settings.vpWidth / 2, settings.vpHeight / 2),
                angle = randAngle,
                speciesID = 0
            };
        }

        // passing agent data + other uniforms
        ComputeUtil.CreateBuffer(ref agentBuffer, agents);
        computeSim.SetBuffer(updateKernel, "slimeAgents", agentBuffer);
        computeSim.SetInt("numAgents", agents.Length);

        ComputeUtil.CreateBuffer(ref speciesBuffer, settings.species);
        computeSim.SetBuffer(updateKernel, "species", speciesBuffer);
        computeSim.SetBuffer(paintKernel, "species", speciesBuffer);       

        computeSim.SetInt("width", settings.vpWidth);
        computeSim.SetInt("height", settings.vpHeight);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < settings.simsPerFrame; i++) 
        {
            Simulate();
        }
    }

    void LateUpdate()
    {
        Graphics.Blit(nextTrailMap, trailMap);
    }

    void Simulate()
    {
        computeSim.SetFloat("dt", Time.fixedDeltaTime);

        // send species related buffers to shader here, for now just using magic values within compute
        computeSim.Dispatch(updateKernel, Mathf.CeilToInt(settings.numAgents / 16.0F), 1, 1);
        computeSim.Dispatch(blurKernel, settings.vpWidth / 8, settings.vpHeight / 8, 1);
        Graphics.Blit(nextTrailMap, viewportTex);
    }

    // Called when the attached Object is destroyed.
    void OnDestroy()
    {
        // Release agent, trail, and color buffers
    }
}
