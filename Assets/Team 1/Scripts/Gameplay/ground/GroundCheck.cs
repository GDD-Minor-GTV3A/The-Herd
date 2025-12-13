using System;
using System.Collections.Generic;

using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Terrain terrain;  // Assign in inspector

    [SerializeField]
    private List<GroundType> groundTypes = new List<GroundType>();

    void Update()
    {
        int textureIndex = GetMainTexture(transform.position);

        GroundType _currentGroundType = groundTypes.Find(x => x.TextureIndex == textureIndex);

        Debug.Log($"Standing on {_currentGroundType.GroundSurface}");
    }

    int GetMainTexture(Vector3 worldPos)
    {
        TerrainData terrainData = terrain.terrainData;

        // Convert world position to terrain map coordinates
        Vector3 terrainPos = worldPos - terrain.transform.position;

        int mapX = Mathf.FloorToInt((terrainPos.x / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = Mathf.FloorToInt((terrainPos.z / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] alphaMap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        int dominant = 0;
        float maxWeight = 0f;

        for (int i = 0; i < alphaMap.Length; i++)
        {
            if (alphaMap[0, 0, i] > maxWeight)
            {
                dominant = i;
                maxWeight = alphaMap[0, 0, i];
            }
        }

        return dominant;
    }
}


[Serializable]
public struct GroundType
{
    [field: SerializeField]
    public int TextureIndex { get; private set; }

    [field: SerializeField]
    public GroundSurface GroundSurface { get; private set; }
}

public enum GroundSurface
{
    Dirt,
    Snow,
    Rock,
    Ice
}