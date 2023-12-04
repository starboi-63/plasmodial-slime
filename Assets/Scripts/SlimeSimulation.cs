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
    
        computeSim.SetTexture(paintKernel, "ViewportTex", viewportTex);
        computeSim.SetTexture(paintKernel, "TrailMap", trailMap);
        
        // clearing trail and viewport textures (setting to 0)
        computeSim.SetTexture(clearKernel, "TrailMap",  trailMap);
        computeSim.Dispatch(clearKernel, settings.vpWidth / 8, settings.vpHeight / 8, 1);

        SlimeAgent[] agents = new SlimeAgent[settings.numAgents];
        for (int i = 0; i < settings.numAgents; i++) {
            float randomTheta = (float)(Random.value) * 2 * Mathf.PI;
            float randomR = (float)(Random.value) * 200;
            float randomOffsetX = Mathf.Cos(randomTheta) * randomR;
            float randomOffsetY = Mathf.Sin(randomTheta) * randomR;
            float randAngle = Mathf.PI + Mathf.Atan2(randomOffsetY, randomOffsetX);
            agents[i] = new SlimeAgent {
                position = new Vector2(settings.vpWidth / 2 + randomOffsetX, settings.vpHeight / 2 + randomOffsetY),
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

        computeSim.SetFloat("decayRate", settings.decayRate);
        computeSim.SetFloat("diffuseRate", settings.diffuseRate);
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
        computeSim.Dispatch(paintKernel, settings.vpWidth / 8, settings.vpHeight / 8, 1);
    }

    void Simulate()
    {
        computeSim.SetFloat("dt", Time.fixedDeltaTime);
        computeSim.SetFloat("time", Time.fixedTime);

        // send species related buffers to shader here, for now just using magic values within compute
        computeSim.Dispatch(updateKernel, Mathf.CeilToInt(settings.numAgents / 16.0F), 1, 1);
        computeSim.Dispatch(blurKernel, settings.vpWidth / 8, settings.vpHeight / 8, 1);
        Graphics.Blit(nextTrailMap, trailMap);
    }


    // Called when the attached Object is destroyed.
    void OnDestroy()
    {
        // Release agent, trail, and color buffers
    }
}
