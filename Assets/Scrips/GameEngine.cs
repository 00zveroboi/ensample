using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public List<SystemBlock> Blocks;

    public SystemBlock AddBlock(int TypeId, List<Vector2> Coords)
    {
        SystemBlock Result = GameObject.Find("GameEngine").AddComponent<SystemBlock>();
        Result.PrecacheBlock(TypeId);
        Result.SetCoords(Coords);
        Blocks.Add(Result);
        return Result;
    }

    public void DeleteBlock(int Index)
    {
        SystemBlock ThisBlock = Blocks[Index];
        ThisBlock.Destroy();
        Blocks.RemoveAt(Index);
        Destroy(ThisBlock);
    }

    void Start()
    {
        TextureData.Precache();
        AddBlock(0, new List<Vector2>(new[] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0) }));
        AddBlock(1, new List<Vector2>(new[] {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1) }));
        AddBlock(2, new List<Vector2>(new[] {
            new Vector2(0, -.5f),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, -.5f) }));
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //    DeleteBlock(0);
    }
}
