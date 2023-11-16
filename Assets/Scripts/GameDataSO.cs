using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data/Create new", order = 0)]
public class GameDataSO : ScriptableObject
{
    [SerializeField] private Vector2Int size;
    [SerializeField] private float tileLength = 1;
    [SerializeField] private float tileHeight = 0.5f;
    [SerializeField] private TileMaterials tileMaterials;

    public Vector2Int Size => size;
    public float TileLength => tileLength;
    public float TileHeight => tileHeight;

    public Material GetTileMaterial(Enums.TileType tileType)
    {
        return tileType switch
        {
            Enums.TileType.Blocked => tileMaterials.tileBlockedMaterial,
            Enums.TileType.ReadyToCheck => tileMaterials.tileReadyToCheckMaterial,
            Enums.TileType.Checked => tileMaterials.tileCheckedMaterial,
            Enums.TileType.Path => tileMaterials.tilePathMaterial,
            Enums.TileType.Default => tileMaterials.tileDefaultMaterial,
            Enums.TileType.Start => tileMaterials.tileStartMaterial,
            Enums.TileType.Destination => tileMaterials.tileDestinationMaterial,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

[Serializable]
public struct TileMaterials
{
    public Material tileDefaultMaterial;
    public Material tileBlockedMaterial;
    public Material tileReadyToCheckMaterial;
    public Material tileCheckedMaterial;
    public Material tilePathMaterial;
    public Material tileStartMaterial;
    public Material tileDestinationMaterial;
}
