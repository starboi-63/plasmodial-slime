using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public ComputeShader computeShader;
    public RawImage rawImage;

    private RenderTexture resultTexture;
    private ComputeBuffer parameterBuffer;
    // Start is called before the first frame update
    void Start()
    {
        resultTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
        resultTexture.enableRandomWrite = true;
        resultTexture.Create();

        rawImage.texture = resultTexture;

        // initialize parameters to compute shader
        parameterBuffer = new ComputeBuffer(1, sizeof(float));

        computeShader.SetTexture(0, "Result", resultTexture);
        computeShader.SetBuffer(0, "_ParameterBuffer", parameterBuffer);

        computeShader.Dispatch(0, resultTexture.width / 8, resultTexture.height / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        float paramValue = 128 * Mathf.Sin(Time.time) + 128;

        float[] paramArray = { paramValue };
        parameterBuffer.SetData(paramArray);

        computeShader.Dispatch(0, resultTexture.width / 8, resultTexture.height / 8, 1);
    }

    void OnDestroy()
    {
        resultTexture.Release();
        parameterBuffer.Release();
    }
}
