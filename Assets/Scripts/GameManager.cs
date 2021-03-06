﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Player Settigns")]
    public Snake playerSnake;
    public bool isGameOver;
    public bool isFirstInput;
    public bool isPowerUpActive;
    int score;
    int highScore;

    [Header("Props Settings")]
    public int pointsToSpawnTrap;
    private int pointsToNextTrap;
    public Color trapColor = Color.black;
    public ParticleSystem particle;
    public Food food;
    public GameObject star;
    [System.NonSerialized]
    public GameObject actualStar;

    [Header("Map Settings")]
    public int mapHeight = 15;
    public int mapWidth = 17;
    public Color mapColor1;
    public Color mapColor2;
    GameObject mapGameObject;
    SpriteRenderer mapRenderer;
    Node[,] mapGrid;
    List<Node> avaliableNodes = new List<Node>();
    List<SpecialNode> traps = new List<SpecialNode>();

    [Header("Camera Settings")]
    public Transform cameraHolder;

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
        DOTween.SetTweensCapacity(3215, 1950);
        onStart.Invoke();
    }

    public void StartNewGame()
    {
        ClearReferences();
        CreateMap();
        playerSnake.CreateSnake();
        PlaceCamera();
        food.CreateFood();
        playerSnake.targetDirection = Snake.Direction.right;
        isGameOver = false;
        score = 0;
        UpdateScoreUI();
        actualStar = Instantiate(star, Vector3.one * 100, Quaternion.identity);
        actualStar.GetComponent<Star>().particle = particle;
        actualStar.SetActive(false);
    }

    public void ClearReferences()
    {
        DeactivatePowerUp();
        if(mapGameObject != null)
            Destroy(mapGameObject);
        if (playerSnake != null)
            Destroy(playerSnake.snakeGameObject);
        if (food.foodCreated)
            Destroy(food.foodGameObject);
        if (actualStar != null)
            Destroy(actualStar);
        foreach (var t in playerSnake.tail)
        {
            if (t != null)
                Destroy(t.nodeGameObject);
        }
        foreach (var item in traps)
        {
            if (item != null)
                Destroy(item.nodeGameObject);
        }
        playerSnake.tail.Clear();
        avaliableNodes.Clear();
        traps.Clear();
        if (playerSnake != null)
            Destroy(playerSnake.tailParent);
        mapGrid = null;
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
        cameraHolder.position = v;
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

    public void RespawnFood()
    {
        food.RandomlyPlaceFood();
    }

    public List<Node> GetAvaliableNodes()
    {
        return avaliableNodes;
    }

    public Node GetFoodNode()
    {
        return food.GetFoodNode();
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
        score += 10;
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

    public void IncreasePointsToNextTrap()
    {
        pointsToNextTrap++;
        TrySpawnTrap();
    }

    public void TrySpawnTrap()
    {
        if(pointsToNextTrap >= pointsToSpawnTrap)
        {
            pointsToNextTrap = 0;
            SpecialNode trap = new SpecialNode();
            trap.nodeGameObject = new GameObject("Trap" + traps.Count);
            int random = Random.Range(0, avaliableNodes.Count);
            trap.node = GetAvaliableNodes()[random];
            traps.Add(trap);
            avaliableNodes.Remove(trap.node);
            PlacePLayerObject(trap.nodeGameObject, trap.node.worldPosition);
            trap.nodeGameObject.transform.localScale = Vector3.zero;
            Tween trapTween = trap.nodeGameObject.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutElastic);
            trapTween.Play();
            SpriteRenderer r = trap.nodeGameObject.AddComponent<SpriteRenderer>();
            r.sortingOrder = 1;
            r.sprite = CreateSprite(trapColor);
            if (isPowerUpActive)
            {
                r.DOFade(0.4f, 0.2f);
            }
        }
    }

    public bool isTrapNode(Node n)
    {
        for (int i = 0; i < traps.Count; i++)
        {
            if (traps[i].node == n)
            {
                return true;
            }
        }
        return false;
    }

    public void SetStarActive()
    {
        actualStar.SetActive(true);
    }

    public void ActivatePowerUp()
    {
        foreach (var tail in playerSnake.tail)
        {
            tail.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(0.4f, 0.2f);
        }
        foreach (var trap in traps)
        {
            trap.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(0.4f, 0.2f);
        }
        actualStar.GetComponent<Star>().PickUpStar();
        isPowerUpActive = true;
    }

    public void DeactivatePowerUp()
    {
        foreach (var tail in playerSnake.tail)
        {
            tail.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(1f, 0f);
        }
        foreach (var trap in traps)
        {
            trap.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(1f, 0f);
        }
        isPowerUpActive = false;
    }

    public void PowerUpEndingWaring()
    {
        foreach (var tail in playerSnake.tail)
        {
            Sequence tailSequence = DOTween.Sequence();
            tailSequence.SetLoops(3);
            tailSequence.Append(tail.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(1f, 0.25f));
            tailSequence.Append(tail.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(0.4f, 0.25f));
            tailSequence.Play();
        }
        foreach (var trap in traps)
        {
            Sequence trapSequence = DOTween.Sequence();
            trapSequence.SetLoops(3);
            trapSequence.Append(trap.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(1f, 0.25f));
            trapSequence.Append(trap.nodeGameObject.GetComponent<SpriteRenderer>().DOFade(0.4f, 0.25f));
            trapSequence.Play();
        }
    }
}
