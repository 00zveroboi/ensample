using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureData : MonoBehaviour
{
    static public List<Material> TextureMaterialItems = new List<Material>();
    static public List<string> TextureNameItems = new List<string>(new[] {
        // 0
        "main",
        // 1
        "main",
        // 2
        "main",

        // last
        "null"});

    static public Material GetMaterial(int Index)
    {
        return TextureMaterialItems[Index];
    }

    static public void Precache()
    {
        for (int I = 0; I < TextureNameItems.Count; I++)
        {
            Material material = new Material(Shader.Find("Diffuse"));
            Object File = Resources.Load("Tiles/" + TextureNameItems[I]);
            material.mainTexture = (Texture2D)File;
            material.mainTextureScale = new Vector2(1f, 1f);
            material.mainTextureOffset = new Vector2(0f, 0f);
            TextureMaterialItems.Add(material);
        }
    }
}
