using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Snake Settigns")]
    public Color snakeColor1;
    public Color snakeColor2;
    [Range(0, 1)] public float moveRate = 0.5f;

    GameObject snakeGameObject;
    GameObject tailParent;
    Node playerCurrentNode;
    List<SpecialNode> tail = new List<SpecialNode>();
    Direction targetDirection;
    Direction currentDirection;
    bool up, left, right, down;
    float timer;
    GameManager gameManager;
    public enum Direction
    {
        up,
        down,
        left,
        right
    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
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
            currentDirection = targetDirection;
            MovePlayer();
        }
    }

    public void CreateSnake()
    {
        snakeGameObject = new GameObject("Snake");
        SpriteRenderer playerRender = snakeGameObject.AddComponent<SpriteRenderer>();
        playerRender.sprite = gameManager.CreateSprite(snakeColor1);
        playerRender.sortingOrder = 1;
        playerCurrentNode = gameManager.GetNode(3, 3);
        gameManager.PlacePLayerObject(snakeGameObject, playerCurrentNode.worldPosition);
        snakeGameObject.transform.localScale = Vector3.one * 1.2f;
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
            SetDirection(Direction.up);
        }
        else if (down)
        {
            SetDirection(Direction.down);
        }
        else if (left)
        {
            SetDirection(Direction.left);
        }
        else if (right)
        {
            SetDirection(Direction.right);
        }
    }
    
    bool isOpposite(Direction d)
    {
        switch (d)
        {
            default:
            case Direction.up:
                if (currentDirection == Direction.down)
                    return true;
                else
                    return false;
            case Direction.down:
                if (currentDirection == Direction.up)
                    return true;
                else
                    return false;
            case Direction.left:
                if (currentDirection == Direction.right)
                    return true;
                else
                    return false;
            case Direction.right:
                if (currentDirection == Direction.left)
                    return true;
                else
                    return false;
        }
    }

    void SetDirection(Direction d)
    {
        if (!isOpposite(d))
        {
            targetDirection = d;
        }
    }

    bool isTailNode(Node n)
    {
        for (int i = 0; i < tail.Count; i++)
        {
            if(tail[i].node == n)
            {
                return true;
            }
        }
        return false;
    }

    void MovePlayer()
    {
        int x = 0;
        int y = 0;

        switch (currentDirection)
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
            if (isTailNode(targetNode))
            {
                //GameOver
            }
            else
            {
                bool score = false;
                if (targetNode == gameManager.GetAppleNode())
                {
                    score = true;
                }

                Node previousNode = playerCurrentNode;
                gameManager.GetAvaliableNodes().Add(previousNode);

                if (score)
                {
                    tail.Add(CreateTailNode(previousNode.x, previousNode.y));
                    gameManager.GetAvaliableNodes().Remove(previousNode);
                }

                MoveTail();
                gameManager.PlacePLayerObject(snakeGameObject, targetNode.worldPosition);
                playerCurrentNode = targetNode;
                gameManager.GetAvaliableNodes().Remove(playerCurrentNode);

                if (score)
                {
                    if (gameManager.GetAvaliableNodes().Count > 0)
                    {
                        gameManager.RandomlyPlaceApple();
                    }
                    else
                    {
                        //You Won
                    }
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
        specialNode.nodeGameObject = new GameObject("Tail " + tail.Count);
        specialNode.nodeGameObject.transform.parent = tailParent.transform;
        specialNode.nodeGameObject.transform.position = specialNode.node.worldPosition;
        specialNode.nodeGameObject.transform.localScale = Vector3.one * .95f;
        SpriteRenderer r = specialNode.nodeGameObject.AddComponent<SpriteRenderer>();
        r.sortingOrder = 1;
        if (tail.Count % 2 == 0)
        {
            r.sprite = gameManager.CreateSprite(snakeColor2);
        }
        else
        {
            r.sprite = gameManager.CreateSprite(snakeColor1);
        }
        return specialNode;

    }

    void MoveTail()
    {
        Node previousNode = null;
        for (int i = 0; i < tail.Count; i++)
        {
            SpecialNode specialNode = tail[i];
            gameManager.GetAvaliableNodes().Add(specialNode.node);

            if(i == 0)
            {
                previousNode = specialNode.node;
                specialNode.node = playerCurrentNode;
            }
            else
            {
                Node prev = specialNode.node;
                specialNode.node = previousNode;
                previousNode = prev;
            }

            gameManager.GetAvaliableNodes().Remove(specialNode.node);
            gameManager.PlacePLayerObject(specialNode.nodeGameObject, specialNode.node.worldPosition);            
        }
    }
}
