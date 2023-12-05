using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ComputeUtilities;
using Unity.VisualScripting;
using TMPro;

public class SlimeSimulation : MonoBehaviour
{
    // UI Vars
    public RawImage viewport;
    public TMP_Text togglePlayText;
    public TMP_Text toggleFoodText;

    public SimulationSettings settings;

    public ComputeShader computeSim;

    const int updateKernel = 0;
    const int blurKernel = 1;
    const int paintKernel = 2;
    const int clearKernel = 3;

    public RenderTexture viewportTex;
    public RenderTexture trailMap;
    public RenderTexture nextTrailMap;
    public RenderTexture foodMap;

    public bool playing = false;
    public bool placingFood = false;

    ComputeBuffer agentBuffer;
    ComputeBuffer speciesBuffer;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize agent, trail, and color buffers
        ComputeUtil.CreateTex(ref viewportTex, settings.vpWidth, settings.vpHeight);
        ComputeUtil.CreateTex(ref trailMap, settings.vpWidth, settings.vpHeight);
        ComputeUtil.CreateTex(ref nextTrailMap, settings.vpWidth, settings.vpHeight);
        ComputeUtil.CreateTex(ref foodMap, settings.vpWidth, settings.vpHeight);

        viewport.texture = viewportTex;

        computeSim.SetTexture(updateKernel, "TrailMap", trailMap);

        computeSim.SetTexture(blurKernel, "TrailMap", trailMap);
        computeSim.SetTexture(blurKernel, "NextTrailMap", nextTrailMap);

        computeSim.SetTexture(paintKernel, "ViewportTex", viewportTex);
        computeSim.SetTexture(paintKernel, "TrailMap", trailMap);
        computeSim.SetTexture(paintKernel, "FoodMap", foodMap);

        // clearing trail, food, and viewport textures (setting to <0,0,0,0>)
        computeSim.SetTexture(clearKernel, "TrailMap", trailMap);
        computeSim.SetTexture(clearKernel, "FoodMap", foodMap);
        computeSim.Dispatch(clearKernel, settings.vpWidth / 8, settings.vpHeight / 8, 1);

        SlimeAgent[] agents = new SlimeAgent[settings.numAgents];
        for (int i = 0; i < settings.numAgents; i++)
        {
            float randomTheta = (float)(Random.value) * 2 * Mathf.PI;
            float randomR = (float)(Random.value) * 250;
            float randomOffsetX = Mathf.Cos(randomTheta) * randomR;
            float randomOffsetY = Mathf.Sin(randomTheta) * randomR;
            float randAngle = Mathf.PI + Mathf.Atan2(randomOffsetY, randomOffsetX);
            agents[i] = new SlimeAgent
            {
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
        computeSim.SetVector("foodColor", settings.foodColor);

        Simulate();
        Paint();
    }

    public void TogglePlaying()
    {
        playing = !playing;

        if (playing)
        {
            togglePlayText.SetText("Pause");
        }
        else
        {
            togglePlayText.SetText("Play");
        }
    }
    public void ToggleFood()
    {
        placingFood = !placingFood;

        if (placingFood)
        {
            toggleFoodText.SetText("Stop");
        }
        else
        {
            toggleFoodText.SetText("Place Food");
        }
    }

    void Update()
    {
        if (placingFood && Input.GetButtonDown("Fire1"))
        {
            PlaceFood();
        }
    }

    void FixedUpdate()
    {
        if (playing)
        {
            for (int i = 0; i < settings.simsPerFrame; i++)
            {
                Simulate();
            }

            Paint();
        }
    }

    void PlaceFood()
    {
        Debug.Log("PlaceFood called");

        Vector2 screenPos = new(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 canvasPos = new();
        bool withinCanvas = RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport.rectTransform, screenPos, null, out canvasPos);

        Debug.Log(withinCanvas);
        Debug.Log(canvasPos);
    }

    void Paint()
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
        // Release buffers
    }
}
