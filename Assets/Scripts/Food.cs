using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("Food Settings")]
    public GameObject foodGameObject;
    public bool foodCreated;
    Node foodNode;

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void CreateFood()
    {
        foodGameObject = Instantiate(foodGameObject);
        foodCreated = true;
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
