using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("Food Settings")]
    public Color foodColor = Color.red;
    [System.NonSerialized]
    public GameObject foodGameObject;
    Node foodNode;

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void CreateFood()
    {
        foodGameObject = new GameObject("Food");
        SpriteRenderer foodRenderer = foodGameObject.AddComponent<SpriteRenderer>();
        foodRenderer.sprite = gm.CreateSprite(foodColor);
        foodRenderer.sortingOrder = 1;
        RandomlyPlaceFood();
    }

    public void RandomlyPlaceFood()
    {
        int random = Random.Range(0, gm.GetAvaliableNodes().Count);
        Node node = gm.GetAvaliableNodes()[random];
        gm.PlacePLayerObject(foodGameObject, node.worldPosition);
        foodNode = node;
    }

    public Node GetFoodNode()
    {
        return foodNode;
    }
}
