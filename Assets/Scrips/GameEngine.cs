using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public List<SystemBlock> Blocks;
    private float WorldRadius, WorldLength;

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

    private List<Vector2> GetRandomRectangle(Vector2 LevelDelta, Vector2 LengthDelta, Vector2 RadiusDelta)
    {
        List<Vector2> Result = new List<Vector2>();
        float PolygonLength = Random.Range(RadiusDelta.x, RadiusDelta.y);
        Vector2 CenterOfPolygon = new Vector2(
            Random.Range(LengthDelta.x, LengthDelta.y), 
            Random.Range(LevelDelta.x, LevelDelta.y));

        // Если полигон не вмещается в уровень его обитания, то обрезаем его.
        if (PolygonLength > LevelDelta.y - LevelDelta.x)
            PolygonLength = LevelDelta.y - LevelDelta.x;

        float PolygonLengthHalf = PolygonLength / 2;
        // Если полигон не влазит снизу, то сдвигаем выше.
        if (CenterOfPolygon.y < LevelDelta.x + PolygonLengthHalf)
            CenterOfPolygon.y = LevelDelta.x + PolygonLengthHalf;
        else
        // Если полигон не влазит сверху, то сдвигаем ниже.
        if (CenterOfPolygon.y > LevelDelta.y - PolygonLengthHalf)
            CenterOfPolygon.y = LevelDelta.y - PolygonLengthHalf;

        Result.AddRange(new[] {
            new Vector2(CenterOfPolygon.x, CenterOfPolygon.y - PolygonLengthHalf),
            new Vector2(CenterOfPolygon.x, CenterOfPolygon.y + PolygonLengthHalf),
            new Vector2(CenterOfPolygon.x, CenterOfPolygon.y + PolygonLengthHalf),
            new Vector2(CenterOfPolygon.x, CenterOfPolygon.y - PolygonLengthHalf)});

        return Result;
    }

    public void CreateNewWorldGenerator(int WorldSizeIndex)
    {
        // Большой.
        if (WorldSizeIndex == 3)
            WorldRadius = 5f;
        else
        // Средний.
        if (WorldSizeIndex == 2)
            WorldRadius = 3f;
        // Маленький.
        else
            WorldRadius = 1f;
        WorldLength = Utils.GetCircleLength(WorldRadius);
        WorldRadius /= 2;

        List<Vector2> ThisPolygon;

        ThisPolygon = GetRandomRectangle(new Vector2(0, WorldRadius * 2 / 3), new Vector2(0, WorldLength), new Vector2(WorldRadius, WorldRadius));
        ThisPolygon[0] = new Vector2(-WorldLength / 2, ThisPolygon[0].y);
        ThisPolygon[1] = new Vector2(-WorldLength / 2, ThisPolygon[1].y);
        ThisPolygon[2] = new Vector2(WorldLength / 2, ThisPolygon[2].y);
        ThisPolygon[3] = new Vector2(WorldLength / 2, ThisPolygon[3].y);
        AddBlock(2, ThisPolygon);

        ThisPolygon = GetRandomRectangle(new Vector2(WorldRadius * 2 / 3, WorldRadius - 0.1f), new Vector2(0, WorldLength), new Vector2(WorldRadius, WorldRadius));
        ThisPolygon[0] = new Vector2(-WorldLength / 2, ThisPolygon[0].y);
        ThisPolygon[1] = new Vector2(-WorldLength / 2, ThisPolygon[1].y);
        ThisPolygon[2] = new Vector2(WorldLength / 2, ThisPolygon[2].y);
        ThisPolygon[3] = new Vector2(WorldLength / 2, ThisPolygon[3].y);
        AddBlock(1, ThisPolygon);

        ThisPolygon = GetRandomRectangle(new Vector2(WorldRadius - 0.1f, WorldRadius), new Vector2(0, WorldLength), new Vector2(WorldRadius, WorldRadius));
        ThisPolygon[0] = new Vector2(-WorldLength / 2, ThisPolygon[0].y);
        ThisPolygon[1] = new Vector2(-WorldLength / 2, ThisPolygon[1].y);
        ThisPolygon[2] = new Vector2(WorldLength / 2, ThisPolygon[2].y);
        ThisPolygon[3] = new Vector2(WorldLength / 2, ThisPolygon[3].y);
        AddBlock(0, ThisPolygon);
    }

    void Start()
    {
        TextureData.Precache();
        CreateNewWorldGenerator(3);
    }

    void Update()
    {
        var CamPos = GameObject.Find("Main Camera").transform.position;
        if (Input.GetKey(KeyCode.A))
            CamPos.x -= 0.025f;
        if (Input.GetKey(KeyCode.D))
            CamPos.x += 0.025f;
        if (Input.GetKey(KeyCode.S))
            CamPos.y -= 0.025f;
        if (Input.GetKey(KeyCode.W))
            CamPos.y += 0.025f;
        if (Input.GetKey(KeyCode.R))
            CamPos.z += 0.025f;
        if (Input.GetKey(KeyCode.F))
            CamPos.z -= 0.025f;
        GameObject.Find("Main Camera").transform.position = CamPos;
    }
}
