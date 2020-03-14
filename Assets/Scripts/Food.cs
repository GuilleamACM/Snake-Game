using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        Sequence foodSequence = DOTween.Sequence();
        foodSequence.SetLoops(-1);
        foodSequence.Append(foodGameObject.transform.DOShakeScale(1, .5f, 10));
        foodSequence.Append(foodGameObject.transform.DOScale(Vector3.one, 0.2f));
        foodSequence.Play();
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
