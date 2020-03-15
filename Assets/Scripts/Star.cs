using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Star : MonoBehaviour
{
    [Header("Star Settings")]
    public float timeToSpawn = 30f;
    public float timeToVanish = 5f;
    public float powerUpTime = 5f;
    public bool isSpawned = false;
    public bool isPowerUpActive = false;
    public ParticleSystem particle;

    private float timeUntilSpawn;
    private float timeUntilVanish;
    private float remaingPowerUpTime;
    SpecialNode star;

    GameManager gm;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        star = new SpecialNode();
        star.nodeGameObject = this.gameObject;
        timeUntilSpawn = timeToSpawn;
        timeUntilVanish = timeToVanish;
        remaingPowerUpTime = powerUpTime;
    }

    void Update()
    {
        if (isSpawned)
            timeUntilVanish -= Time.deltaTime;
        else
            timeUntilSpawn -= Time.deltaTime;

        if (isPowerUpActive)
        {
            if (remaingPowerUpTime <= 0f)
                DeactivatePowerUp();
            remaingPowerUpTime -= Time.deltaTime;
        }

        if (timeUntilSpawn <= 0f)
            SpawnStar();

        if (timeUntilVanish <= 0f)
            VanishStar();

    }

    void SpawnStar()
    {
        int random = Random.Range(0, gm.GetAvaliableNodes().Count);
        star.node = gm.GetAvaliableNodes()[random];
        gm.GetAvaliableNodes().Remove(star.node);
        this.transform.localScale = Vector3.zero;
        gm.PlacePLayerObject(star.nodeGameObject, star.node.worldPosition);
        gm.PlacePLayerObject(particle.gameObject, star.node.worldPosition);
        Tween starTween = this.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutElastic);
        starTween.Play();
        particle.Play();
        Sequence starSequence = DOTween.Sequence();
        starSequence.SetLoops(-1);
        starSequence.Append(this.transform.DOShakeScale(1, .5f, 10));
        starSequence.Append(this.transform.DOScale(Vector3.one, 0.2f));
        starSequence.Play();
        timeUntilSpawn = timeToSpawn;
        isSpawned = true;

    }

    void VanishStar()
    {
        this.gameObject.transform.position = Vector3.one * 100;
        gm.GetAvaliableNodes().Add(star.node);
        star.node = null;
        timeUntilVanish = timeToVanish;
        isSpawned = false;
    }

    public bool isStarNode (Node n)
    {
        if (star.node != null && star.node == n)
            return true;

        return false;

    }

    public void PickUpStar()
    {
        VanishStar();
        isPowerUpActive = true;
    }
    
    public void DeactivatePowerUp()
    {
        gm.DeactivatePowerUp();
        remaingPowerUpTime = powerUpTime;
        isPowerUpActive = false;
    }

}
