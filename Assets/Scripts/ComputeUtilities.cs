namespace ComputeUtilities
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using UnityEngine;
	using UnityEngine.Experimental.Rendering;

    public static class ComputeUtil
    {
        public static void CreateTex(ref RenderTexture tex, int width, int height) 
        {
            tex = new RenderTexture(width, height, 0);
            tex.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
            tex.enableRandomWrite = true;
            tex.autoGenerateMips = false;
            tex.Create();

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear; // probably change this to bilinear at some point
        }

        public static void CreateBuffer<T>(ref ComputeBuffer buff, T[] data) {
            if (buff != null)
            {
                buff.Release();
            }
            
            int count = data.Length;
            int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            buff = new ComputeBuffer(count, stride);
            buff.SetData(data);
        }
    }
}
