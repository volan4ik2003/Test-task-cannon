using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSetup : MonoBehaviour
{
    public Material planeMaterial;
    public int textureWidth = 1024;
    public int textureHeight = 1024;
    private RenderTexture renderTexture;

    void Start()
    {
        renderTexture = new RenderTexture(textureWidth, textureHeight, 0);
        renderTexture.Create();

        planeMaterial.mainTexture = renderTexture;

        GetComponent<Renderer>().material = planeMaterial;

        Texture2D baseTexture = planeMaterial.GetTexture("_BaseTex") as Texture2D;
        if (baseTexture != null)
        {
            Graphics.Blit(baseTexture, renderTexture);
        }
    }

    public RenderTexture GetRenderTexture()
    {
        return renderTexture;
    }
}
