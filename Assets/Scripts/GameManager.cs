using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Map Settings")]
    public int mapHeight = 15;
    public int mapWidth = 17;
    public Color mapColor1;
    public Color mapColor2;

    GameObject mapGameObject;
    SpriteRenderer mapRenderer;
    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
    }

    void CreateMap()
    {
        mapGameObject = new GameObject("Map");
        mapRenderer = mapGameObject.AddComponent<SpriteRenderer>();

        Texture2D mapTexture = new Texture2D(mapWidth, mapHeight);
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                #region Map Visual
                if (x % 2 != 0)
                {
                    if (y % 2 != 0)
                    {
                        mapTexture.SetPixel(x, y, mapColor1);
                    }
                    else
                    {
                        mapTexture.SetPixel(x, y, mapColor2);
                    }
                }
                else
                {
                    if (y % 2 != 0)
                    {
                        mapTexture.SetPixel(x, y, mapColor2);
                    }
                    else
                    {
                        mapTexture.SetPixel(x, y, mapColor1);
                    }
                }
                #endregion
            }
        }
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.Apply();
        Rect rect = new Rect(0, 0, mapWidth, mapHeight);
        Sprite sprite = Sprite.Create(mapTexture, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
        mapRenderer.sprite = sprite;
    }
}
