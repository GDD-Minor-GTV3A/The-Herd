using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;  // Assign in inspector

    [SerializeField]
    private List<GroundType> groundTypes = new List<GroundType>();

    [SerializeField]
    private GroundType currentGroundType;

    void Update()
    {
        int textureIndex = GetMainTexture(transform.position);

        GroundType _currentGroundType = groundTypes.Find(x => x.TextureIndex == textureIndex);
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
    public GroundSurface GetGroundType()
    {
        int textureIndex = GetMainTexture(transform.position);

        foreach (var groundType in groundTypes)
        {
            if (groundType.TextureIndex == textureIndex)
            {
                currentGroundType = groundType;
                return groundType.GroundSurface;
            }
        }
        Debug.LogWarning($"No GroundType found for texture index {textureIndex}");
        return GroundSurface.Dirt;
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
}