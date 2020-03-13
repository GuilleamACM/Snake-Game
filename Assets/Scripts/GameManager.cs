using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Player Settigns")]
    public Color playerColor;
    [Range(0, 1)]public float moveRate = 0.5f;
    GameObject playerGameObject;
    Node playerCurrentNode;
    Direction playerCurrentDirection;
    bool up, left, right, down;

    float timer;

    [Header("Map Settings")]
    public int mapHeight = 15;
    public int mapWidth = 17;
    public Color mapColor1;
    public Color mapColor2;

    public Transform camera;
    GameObject mapGameObject;
    SpriteRenderer mapRenderer;
    Node[,] mapGrid;

    public enum Direction
    {
        up,
        down,
        left,
        right
    }
    #region Initialization
    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
        PlacePlayer();
        PlaceCamera();
        playerCurrentDirection = Direction.right;
    }

    void PlacePlayer()
    {
        playerGameObject = new GameObject("Player");
        SpriteRenderer playerRender = playerGameObject.AddComponent<SpriteRenderer>();
        playerRender.sprite = CreateSprite(playerColor);
        playerRender.sortingOrder = 1;
        playerCurrentNode = GetNode(3, 3);
        playerGameObject.transform.position = playerCurrentNode.worldPosition;
    }

    void CreateMap()
    {
        mapGameObject = new GameObject("Map");
        mapRenderer = mapGameObject.AddComponent<SpriteRenderer>();
        mapGrid = new Node[mapWidth, mapHeight];
        Texture2D mapTexture = new Texture2D(mapWidth, mapHeight);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 vector = Vector3.zero;
                vector.x = x;
                vector.y = y;
                Node node = new Node()
                {
                    x = x,
                    y = y,
                    worldPosition = vector
                };
                mapGrid[x, y] = node;

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
        Sprite sprite = Sprite.Create(mapTexture, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
        mapRenderer.sprite = sprite;
    }

    void PlaceCamera()
    {
        Node n = GetNode(mapWidth / 2, mapHeight / 2);
        Vector3 v = n.worldPosition;
        v += Vector3.one * .5f;
        camera.position = v;
    }
    #endregion

    void GetInput()
    {
        up = Input.GetButtonDown("Up");
        down = Input.GetButtonDown("Down");
        left = Input.GetButtonDown("Left");
        right = Input.GetButtonDown("Right");
    }

    void SetPlayerDirection()
    {
        if (up)
        {
            playerCurrentDirection = Direction.up;
        }
        else if (down)
        {
            playerCurrentDirection = Direction.down;
        }
        else if (left)
        {
            playerCurrentDirection = Direction.left;
        }
        else if (right)
        {
            playerCurrentDirection = Direction.right;
        }
    }

    void MovePlayer()
    {
        int x = 0;
        int y = 0;

        switch (playerCurrentDirection)
        {
            case Direction.up:
                y = 1;
                break;
            case Direction.down:
                y = -1;
                break;
            case Direction.left:
                x = -1;
                break;
            case Direction.right:
                x = 1;
                break;
        }

        Node targetNode = GetNode(playerCurrentNode.x + x, playerCurrentNode.y + y);
        if (targetNode != null)
        {
            playerGameObject.transform.position = targetNode.worldPosition;
            playerCurrentNode = targetNode;
        }
        else
        {
            //Game Over
        }
    }

    void Update()
    {
        GetInput();
        SetPlayerDirection();

        timer += Time.deltaTime;
        if(timer > moveRate)
        {
            timer = 0;
            MovePlayer();
        }
    }

    #region Utilities
    Sprite CreateSprite(Color targerColor)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.filterMode = FilterMode.Point;
        texture.SetPixel(0, 0, targerColor);
        texture.Apply();
        Rect rect = new Rect(0, 0, 1, 1);
        return Sprite.Create(texture, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
    }

    Node GetNode(int x, int y)
    {
        if(x < 0 || x > mapWidth - 1 || y < 0 || y > mapHeight - 1)
        {
            return null;
        }
        return mapGrid[x, y];
    }
    #endregion
}
