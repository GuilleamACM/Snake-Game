using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Player Settigns")]
    public Color snakeColor1;
    public Color snakeColor2;
    [Range(0, 1)] public float moveRate = 0.5f;

    GameObject snakeGameObject;
    GameObject tailParent;
    Node playerCurrentNode;
    List<SpecialNode> tail = new List<SpecialNode>();
    Direction playerCurrentDirection;
    bool up, left, right, down;
    float timer;
    int snakeLength = 0;
    GameManager gameManager;
    public enum Direction
    {
        up,
        down,
        left,
        right
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void CreateSnake()
    {
        snakeGameObject = new GameObject("Snake");
        SpriteRenderer playerRender = snakeGameObject.AddComponent<SpriteRenderer>();
        playerRender.sprite = gameManager.CreateSprite(snakeColor1);
        playerRender.sortingOrder = 1;
        playerCurrentNode = gameManager.GetNode(3, 3);
        snakeGameObject.transform.position = playerCurrentNode.worldPosition;
        tailParent = new GameObject("tailParent");
    }

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

        Node targetNode = gameManager.GetNode(playerCurrentNode.x + x, playerCurrentNode.y + y);
        if (targetNode != null)
        {
            bool score = false;
            if (targetNode == gameManager.GetAppleNode())
            {
                score = true;
            }

            gameManager.GetAvaliableNodes().Remove(playerCurrentNode);
            snakeGameObject.transform.position = targetNode.worldPosition;
            playerCurrentNode = targetNode;
            gameManager.GetAvaliableNodes().Add(playerCurrentNode);

            //Move tail

            if (score)
            {
                if (gameManager.GetAvaliableNodes().Count > 0)
                {
                    snakeLength++;
                    gameManager.RandomlyPlaceApple();
                }
                else
                {
                    //You Won
                }
            }
        }
        else
        {
            //Game Over
        }
    }

    SpecialNode CreateTailNode(int x, int y)
    {
        SpecialNode specialNode = new SpecialNode();
        specialNode.node = gameManager.GetNode(x, y);
        specialNode.nodeGameObject = new GameObject();
        specialNode.nodeGameObject.transform.parent = tailParent.transform;
        specialNode.nodeGameObject.transform.position = specialNode.node.worldPosition;
        SpriteRenderer r = specialNode.nodeGameObject.AddComponent<SpriteRenderer>();
        if (snakeLength % 2 == 0)
        {
            r.sprite = gameManager.CreateSprite(snakeColor2);
        }
        else
        {
            r.sprite = gameManager.CreateSprite(snakeColor1);
        }
        return specialNode;

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        SetPlayerDirection();

        timer += Time.deltaTime;
        if (timer > moveRate)
        {
            timer = 0;
            MovePlayer();
        }
    }
}
