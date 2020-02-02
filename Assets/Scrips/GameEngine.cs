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
        float PolygonLength = Random.Range(LengthDelta.x, LengthDelta.y);
        float PolygonRadius = Random.Range(RadiusDelta.x, RadiusDelta.y);
        Vector2 CenterOfPolygon = new Vector2(0,
            Random.Range(LevelDelta.x, LevelDelta.y));

        // Если полигон не вмещается в уровень его обитания, то обрезаем его.
        if (PolygonRadius > LevelDelta.y - LevelDelta.x)
            PolygonRadius = LevelDelta.y - LevelDelta.x;

        float PolygonLengthHalf = PolygonLength / 2;
        float PolygonRadiusHalf = PolygonRadius / 2;
        // Если полигон не влазит снизу, то сдвигаем выше.
        if (CenterOfPolygon.y < LevelDelta.x + PolygonRadiusHalf)
            CenterOfPolygon.y = LevelDelta.x + PolygonRadiusHalf;
        else
        // Если полигон не влазит сверху, то сдвигаем ниже.
        if (CenterOfPolygon.y > LevelDelta.y - PolygonRadiusHalf)
            CenterOfPolygon.y = LevelDelta.y - PolygonRadiusHalf;

        Result.AddRange(new[] {
            new Vector2(CenterOfPolygon.x - PolygonLengthHalf, CenterOfPolygon.y - PolygonRadiusHalf),
            new Vector2(CenterOfPolygon.x - PolygonLengthHalf, CenterOfPolygon.y + PolygonRadiusHalf),
            new Vector2(CenterOfPolygon.x + PolygonLengthHalf, CenterOfPolygon.y + PolygonRadiusHalf),
            new Vector2(CenterOfPolygon.x + PolygonLengthHalf, CenterOfPolygon.y - PolygonRadiusHalf)});

        return Result;
    }

    private void MovePolygon(ref List<Vector2> Polygon, Vector2 Move)
    {
        for (int I = 0; I < Polygon.Count; I++)
            Polygon[I] += Move;
    }

    private void AddPolygonPointAtIndex(ref List<Vector2> Polygon, Vector2 Point, int Index)
    {
        if (Index >= 0 && Index <= Polygon.Count)
            Polygon.Insert(Index, Point);
    }

    private void SetPolygonPointAtIndex(ref List<Vector2> Polygon, Vector2 Point, int Index)
    {
        if (Index >= 0 && Index < Polygon.Count)
            Polygon[Index] = Point;
    }

    private Vector2 GetPolygonPointByIndex(List<Vector2> Polygon, int Index)
    {
        if (Index >= 0 && Index < Polygon.Count)
            return Polygon[Index];
        else
            return new Vector2(0, 0);
    }

    private void AddPolygonToMainBlocks(ref List<Vector2> Polygon1, ref List<Vector2> Polygon2, 
        Vector2 Point, int Index1, int Index2)
    {
        AddPolygonPointAtIndex(ref Polygon1, Point, Index1);
        AddPolygonPointAtIndex(ref Polygon2, Point, Index2);
    }

    private void InversionPolygon(ref List<Vector2> Polygon, int FromIndex, int ToIndex, int StartInversionIndex)
    {
        if (StartInversionIndex < FromIndex)
        {
            Vector2 Height = new Vector2(0, 
                GetPolygonPointByIndex(Polygon, 1).y - GetPolygonPointByIndex(Polygon, 0).y);
            int Phase = 0;
            for (int I = FromIndex; I <= ToIndex; I++)
                AddPolygonPointAtIndex(ref Polygon,
                    GetPolygonPointByIndex(Polygon, I + (Phase++)) + Height,
                    StartInversionIndex);
            SetPolygonPointAtIndex(ref Polygon,
                GetPolygonPointByIndex(Polygon, StartInversionIndex + Phase + 1) + Height,
                StartInversionIndex + Phase);
        }
    }

    private void NormalizingPolygon(ref List<Vector2> Polygon)
    {
        if (Polygon.Count < 4)
            return;
        int CenterIndex;
        Vector2 DiffVectorLeft, VectorCenter, DiffVectorRight;
        for (int I = 0; I < Polygon.Count; I++)
        {
            CenterIndex = (I + 1) % Polygon.Count;
            VectorCenter = Polygon[CenterIndex];
            DiffVectorLeft = VectorCenter - Polygon[I];
            DiffVectorRight = Polygon[(I + 2) % Polygon.Count] - VectorCenter;
            if ((DiffVectorRight.x == 0 && DiffVectorLeft.x == 0) ||
                (DiffVectorRight.y == 0 && DiffVectorLeft.y == 0) ||
                DiffVectorRight.x / DiffVectorLeft.x == DiffVectorRight.y / DiffVectorLeft.y)
            {
                Polygon.RemoveAt(CenterIndex);
                I--;
            }
        }

        for (int I = 0; I < Polygon.Count; I++)
            if (Polygon[I] == Polygon[(I + 2) % Polygon.Count])
                Polygon.RemoveAt((I + 1) % Polygon.Count);

        for (int I = 0; I < Polygon.Count; I++)
            if (Polygon[I] == Polygon[(I + 1) % Polygon.Count])
            {
                Polygon.RemoveAt(I);
                I--;
            }
    }

    private bool MergePolygons(ref List<Vector2> MainPolygon, List<Vector2> AddPolygon)
    {
        int AddPolygonCount = AddPolygon.Count,
            MainPolygonCount = MainPolygon.Count,
            mI, mJ;
        for (int I = 0; I < MainPolygonCount; I++)
        {
            mI = Utils.Mod(I + 1, MainPolygonCount);
            for (int J = 0; J < AddPolygonCount; J++)
            {
                mJ = Utils.Mod(J + 1, AddPolygonCount);
                if (MainPolygon[I] == AddPolygon[mJ] && MainPolygon[mI] == AddPolygon[J])
                {
                    for (int K = J + 2; K < J + AddPolygonCount; K++)
                        AddPolygonPointAtIndex(ref MainPolygon, AddPolygon[Utils.Mod(K, AddPolygonCount)], I + K - 1 - J);
                    NormalizingPolygon(ref MainPolygon);
                    return true;
                }
            }
        }
        return false;
    }

    private void ApplyGeneratorMainRelief(ref List<Vector2> Polygon1, ref List<Vector2> Polygon2, 
        int Index1, int Index2, Vector2Int CountDelta, float MaxLength, float Interval, 
        Vector3 PhaseY, float FromY, float FromX, bool Inversion)
    {
        int CountRnd = Random.Range(CountDelta.x, CountDelta.y);
        float PolygonizeStep = MaxLength / (CountRnd + 1),
            PolygonizeRangeRnd = PolygonizeStep * Interval,
            XRnd, YRnd = FromY;
        Vector2 PolygonRnd;
        for (int I = 0; I < CountRnd; I++)
        {
            XRnd = FromX + Random.Range(
                (I + 1) * PolygonizeStep - PolygonizeRangeRnd,
                (I + 1) * PolygonizeStep + PolygonizeRangeRnd);
            YRnd += Random.Range(PhaseY.x, PhaseY.y);
            if (Mathf.Abs(YRnd - FromY) > PhaseY.z)
                YRnd = FromY + Mathf.Sign(YRnd - FromY) * PhaseY.z;
            PolygonRnd = new Vector2(XRnd, YRnd);
            AddPolygonToMainBlocks(ref Polygon1, ref Polygon2, PolygonRnd, Index1 + I, Index2);
        }
        SetPolygonPointAtIndex(ref Polygon1, new Vector2(FromX + MaxLength, YRnd), Index1 + CountRnd);
        SetPolygonPointAtIndex(ref Polygon2, new Vector2(FromX + MaxLength, YRnd), Index2 - 1);
        if (Inversion)
            InversionPolygon(ref Polygon2, Index2, Index2 + CountRnd - 1, 2);
    }

    private bool IsNormalPolygon(List<Vector2> Polygon)
    {
        int PolygonCount = Polygon.Count;
        if (PolygonCount > 3)
        {
            Vector2[] Line1, Line2;
            float Angle, Angle1, Angle2;
            for (int I = 0; I < PolygonCount; I++)
                for (int J = I + 2; J < PolygonCount; J++)
                {
                    Line1 = new Vector2[2] { 
                        Polygon[I], 
                        Polygon[(I + 1) % PolygonCount] 
                    };
                    Line2 = new Vector2[2] { 
                        Polygon[J], 
                        Polygon[(J + 1) % PolygonCount] 
                    };
                    Angle = Utils.GetAngleByVector(Line1[1] - Line1[0]);
                    Angle1 = Utils.GetAngleByVector(Line2[0] - Line1[0]);
                    Angle2 = Utils.GetAngleByVector(Line2[1] - Line1[0]);
                    if (Mathf.Abs(Utils.GetAngleBetween(Angle, Angle1) + Utils.GetAngleBetween(Angle, Angle2) - Utils.GetAngleBetween(Angle1, Angle2)) < 0.01)
                    {
                        Angle = Utils.GetAngleByVector(Line1[0] - Line1[1]);
                        Angle1 = Utils.GetAngleByVector(Line2[0] - Line1[1]);
                        Angle2 = Utils.GetAngleByVector(Line2[1] - Line1[1]);
                        if (Mathf.Abs(Utils.GetAngleBetween(Angle, Angle1) + Utils.GetAngleBetween(Angle, Angle2) - Utils.GetAngleBetween(Angle1, Angle2)) < 0.01)
                            return false;
                    }
                }
        }
        return true;
    }

    private void CutBlock(ref List<Vector2> Polygon)
    {
        int LeftIndex, RightIndex, CutChance = 75, NextCutChanceMinus = 16;
        Vector2 LeftPoint, RightPoint, ThisPoint, LeftDiff, RightDiff;
        for (int I = 0; I < Polygon.Count; I++)
            if (Random.Range(1, 100) <= CutChance)
            {
                LeftIndex = Utils.Mod(I - 1, Polygon.Count);
                LeftPoint = Polygon[LeftIndex];
                RightIndex = Utils.Mod(I + 1, Polygon.Count);
                RightPoint = Polygon[RightIndex];
                ThisPoint = Polygon[I];
                LeftDiff = (LeftPoint - ThisPoint) * Random.Range(.25f, .75f);
                RightDiff = (RightPoint - ThisPoint) * Random.Range(.25f, .75f);

                AddPolygonPointAtIndex(ref Polygon, ThisPoint + LeftDiff, I);
                SetPolygonPointAtIndex(ref Polygon, ThisPoint + LeftDiff + RightDiff, ++I);
                AddPolygonPointAtIndex(ref Polygon, ThisPoint + RightDiff, ++I);

                I -= Random.Range(0, 2);
                CutChance -= NextCutChanceMinus;
                NextCutChanceMinus -= 2;
                if (NextCutChanceMinus <= 0)
                    break;
            }
    }

    private void CircleBlock(ref List<Vector2> Polygon)
    {
        int LeftIndex, RightIndex, PolygonCount = Polygon.Count;
        Vector2 LeftPoint, RightPoint, ThisPoint, LeftDiff, RightDiff;
        List<Vector2> TempRectangle = new List<Vector2>(Polygon.ToArray());
        for (int I = 0; I < PolygonCount; I++)
        {
            LeftIndex = Utils.Mod(I - 1, PolygonCount);
            LeftPoint = Polygon[LeftIndex];
            RightIndex = Utils.Mod(I + 1, PolygonCount);
            RightPoint = Polygon[RightIndex];
            ThisPoint = Polygon[I];
            LeftDiff = LeftPoint - ThisPoint;
            RightDiff = RightPoint - ThisPoint;

            AddPolygonPointAtIndex(ref TempRectangle, ThisPoint + LeftDiff * (1 - Mathf.Cos(Mathf.PI / 4)) + RightDiff * (1 - Mathf.Sin(Mathf.PI / 3)) * .25f, I * 4);
            SetPolygonPointAtIndex(ref TempRectangle, ThisPoint + LeftDiff * (1 - Mathf.Sin(Mathf.PI / 4)) * .5f + RightDiff * (1 - Mathf.Cos(Mathf.PI / 4)) * .5f, I * 4 + 1);
            AddPolygonPointAtIndex(ref TempRectangle, ThisPoint + LeftDiff * (1 - Mathf.Sin(Mathf.PI / 3)) * .25f + RightDiff * (1 - Mathf.Cos(Mathf.PI / 4)), I * 4 + 2);
            AddPolygonPointAtIndex(ref TempRectangle, ThisPoint + LeftDiff * .0f + RightDiff * .5f, I * 4 + 3);
        }
        Polygon = TempRectangle;
    }

    private void ApplyGeneratorSharpenBlock(ref List<Vector2> Polygon)
    {
        List<Vector2> TempPolygon;
        do
        {
            TempPolygon = new List<Vector2>(Polygon.ToArray());
            CutBlock(ref TempPolygon);
        }
        while (!IsNormalPolygon(TempPolygon));
        Polygon = TempPolygon;
        CircleBlock(ref Polygon);
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

        List<Vector2> StonePolygon = GetRandomRectangle(
            new Vector2(0, WorldRadius * 2 / 3), 
            new Vector2(WorldLength, WorldLength), 
            new Vector2(WorldRadius, WorldRadius));
        List<Vector2> DirtPolygon = GetRandomRectangle(
            new Vector2(WorldRadius * 2 / 3, WorldRadius - 0.1f), 
            new Vector2(WorldLength, WorldLength), 
            new Vector2(WorldRadius, WorldRadius));
        List<Vector2> GrassPolygon = GetRandomRectangle(
            new Vector2(WorldRadius - 0.1f, WorldRadius), 
            new Vector2(WorldLength, WorldLength), 
            new Vector2(WorldRadius, WorldRadius));

        ApplyGeneratorMainRelief(ref StonePolygon, ref DirtPolygon, 2, 4, 
            new Vector2Int((int)(WorldRadius * 600), (int)(WorldRadius * 700)), WorldLength, 0.1f, 
            new Vector3(-WorldRadius / 300, WorldRadius / 300, WorldRadius / 10), 
            WorldRadius * 2 / 3, -WorldLength / 2, false);
        ApplyGeneratorMainRelief(ref DirtPolygon, ref GrassPolygon, 2, 4,
            new Vector2Int((int)(WorldRadius * 600), (int)(WorldRadius * 700)), WorldLength, 0.1f,
            new Vector3(-WorldRadius / 500, WorldRadius / 500, 0.1f),
            WorldRadius - 0.1f, -WorldLength / 2, true);

        AddBlock(2, StonePolygon);
        AddBlock(1, DirtPolygon);
        AddBlock(0, GrassPolygon);
        
        List<Vector2> ThisPolygon;

        for (int I = 1; I <= 120; I++)
        {
            ThisPolygon = GetRandomRectangle(
                new Vector2(0, WorldRadius * 2 / 3),
                new Vector2(0.1f, 0.3f),
                new Vector2(0.1f, 0.3f));
            ApplyGeneratorSharpenBlock(ref ThisPolygon);
            MovePolygon(ref ThisPolygon, new Vector2(Random.Range(-WorldLength / 2, WorldLength / 2), 0));
            AddBlock(0, ThisPolygon);
        }
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
