using UnityEngine;

public class MapPlayerPosition : MonoBehaviour
{
    public Transform player;                 // player transform in the world
    public RectTransform mapRect;            // UI map image
    public RectTransform playerIcon;         // icon on map
    public FadeChildrenImages[] areas;
    public float startxoffset = 0;                 // starting x position of the player icon on the map
    public float startyoffset = 0;                 // starting y position of the player icon on the map
    // world boundaries that correspond to the edges of the map
    public Vector2 worldMin;                 // (minX, minZ)
    public Vector2 worldMax;                 // (maxX, maxZ)
    private int counter = 0;
    void Update()
    {
        counter++;
        UpdatePlayerIconPosition();

    }

    void UpdatePlayerIconPosition()
    {
        Vector3 pos = player.position;

        float normX = Mathf.InverseLerp(worldMin.x, worldMax.x, pos.x);
        float normY = Mathf.InverseLerp(worldMin.y, worldMax.y, pos.z);

        float mapX = (normX * mapRect.rect.width) - (mapRect.rect.width / 2f);
        float mapY = (normY * mapRect.rect.height) - (mapRect.rect.height / 2f);

        float rotatedX = mapX - startxoffset;
        float rotatedY = mapY - startyoffset;


        playerIcon.anchoredPosition = new Vector2(rotatedX, rotatedY);
        
        if (counter >= 10)
        {
            checkAreaFade(rotatedX, rotatedY);
            counter = 0;
        }
    }
    private void checkAreaFade(float x, float y)
    {
        foreach (var area in areas)
        {
            if (area.checkForReveal(x, y))
            {
                area.FadeIn();     // reveal
            } 
        }
    }


}