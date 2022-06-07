using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class McPlanetTower : McUnit
{
    
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetDie,OnPlanetDie);
        canBeTarget = false;
    }

    protected override void Start()
    {
        base.Start();
        transform.DOMove(transform.position + Vector3.up * 10f, 5f);
    }


    void OnPlanetDie(Planet diePlanet)
    {
        if (diePlanet == ownerPlanet)
        {
            Die();
        }
    }
}
