using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Terrain terrain;  // Assign in inspector

    void Update()
    {
        int textureIndex = GetMainTexture(transform.position);

        Debug.Log("Standing on texture index: " + textureIndex);
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
