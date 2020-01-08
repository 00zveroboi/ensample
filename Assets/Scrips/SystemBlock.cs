using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemBlock : MonoBehaviour
{
    static private int BlocksId = 1;
    public int Id;
    private int BlockTypeId;
    private GameObject TextureBlock;
    private List<Vector2> Coords;

    public void PrecacheBlock(int aBlockTypeId)
    {
        this.Id = BlocksId++;
        this.BlockTypeId = aBlockTypeId;
        TextureBlock = new GameObject("Block");
        TextureBlock.AddComponent<MeshFilter>();
        MeshRenderer Renderer = TextureBlock.AddComponent<MeshRenderer>();
        TextureBlock.AddComponent<Block>();
        Renderer.material = TextureData.GetMaterial(aBlockTypeId);
    }

    public void SetCoords(List<Vector2> aCoords)
    {
        this.Coords = aCoords;
        TextureBlock.GetComponent<Block>().DoShow(aCoords);
    }

    public List<Vector2> GetCoords()
    {
        return this.Coords;
    }

    public int GetBlockTypeId()
    {
        return this.BlockTypeId;
    }

    public void Destroy()
    {
        Destroy(TextureBlock);
    }
}
