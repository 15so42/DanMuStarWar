using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class McPlanetTower : McUnit
{

    public float height = 10;
    [Header("覆盖父级变量canBeTarget")]
    public bool canBeAttack=false;//可被选中
    
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetDie,OnPlanetDie);
        canBeTarget = false;
        if (canBeAttack)
        {
            canBeTarget = true;
        }
    }

    protected override void Start()
    {
        base.Start();
        transform.DOMove(transform.position + Vector3.up * height, 5f);
    }


    void OnPlanetDie(Planet diePlanet)
    {
        if (diePlanet == ownerPlanet)
        {
            Die();
        }
    }
}
