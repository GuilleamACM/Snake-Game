using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player Settigns")]
    public Snake playerSnake;
    public bool isGameOver;
    public bool isFirstInput;
    int score;
    int highScore;

    [Header("Props Settings")]
    public Color appleColor = Color.red;
    GameObject appleGameObject;
    Node appleNode;

    [Header("Map Settings")]
    public int mapHeight = 15;
    public int mapWidth = 17;
    public Color mapColor1;
    public Color mapColor2;
    GameObject mapGameObject;
    SpriteRenderer mapRenderer;
    Node[,] mapGrid;
    List<Node> avaliableNodes = new List<Node>();

    [Header("Camera Settings")]
    public Transform camera;

    [Header("UI Settings")]
    public Text scoreText;
    public Text highScoreText;

    [Header("Game Events")]
    public UnityEvent onStart;
    public UnityEvent onScore;
    public UnityEvent firstInput;
    public UnityEvent onGameOver;

    // Start is called before the first frame update
    void Start()
    {
        onStart.Invoke();
    }

    public void StartNewGame()
    {
        ClearReferences();
        CreateMap();
        playerSnake.CreateSnake();
        PlaceCamera();
        CreateApple();
        playerSnake.targetDirection = Snake.Direction.right;
        isGameOver = false;
        score = 0;
        UpdateScoreUI();
    }

    public void ClearReferences()
    {
        if(mapGameObject != null)
            Destroy(mapGameObject);
        if (playerSnake != null)
            Destroy(playerSnake.snakeGameObject);
        if (appleGameObject != null)
            Destroy(appleGameObject);
        foreach (var t in playerSnake.tail)
        {
            if (t != null)
                Destroy(t.nodeGameObject);
        }
        playerSnake.tail.Clear();
        avaliableNodes.Clear();
        if (playerSnake != null)
            Destroy(playerSnake.tailParent);
        mapGrid = null;
    }

    void CreateApple()
    {
        appleGameObject = new GameObject("Apple");
        SpriteRenderer appleRenderer = appleGameObject.AddComponent<SpriteRenderer>();
        appleRenderer.sprite = CreateSprite(appleColor);
        appleRenderer.sortingOrder = 1;
        RandomlyPlaceApple();
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
                avaliableNodes.Add(node);
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

    public Sprite CreateSprite(Color targerColor)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.filterMode = FilterMode.Point;
        texture.SetPixel(0, 0, targerColor);
        texture.Apply();
        Rect rect = new Rect(0, 0, 1, 1);
        return Sprite.Create(texture, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
    }

    public void RandomlyPlaceApple()
    {
        int random = Random.Range(0, avaliableNodes.Count);
        Node node = avaliableNodes[random];
        PlacePLayerObject(appleGameObject, node.worldPosition);
        appleNode = node;
    }

    public List<Node> GetAvaliableNodes()
    {
        return avaliableNodes;
    }

    public Node GetAppleNode()
    {
        return appleNode;
    }

    public Node GetNode(int x, int y)
    {
        if(x < 0 || x > mapWidth - 1 || y < 0 || y > mapHeight - 1)
        {
            return null;
        }
        return mapGrid[x, y];
    }

    public void PlacePLayerObject(GameObject obj, Vector3 pos)
    {
        pos += Vector3.one * .5f;
        obj.transform.position = pos;
    }

    public void IncreaseScore()
    {
        score++;
        if (score > highScore)
        {
            highScore = score;
        }
        onScore.Invoke();
    }

    public void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
        highScoreText.text = highScore.ToString();
    }

    public void GameOver()
    {
        isGameOver = true;
        isFirstInput = false;
    }
}
