using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;
[IncludeInSettings(true)]
public class BattleUnit : MonoBehaviour
{
   
    public Planet ownerPlanet;

    private StateMachine stateMachine;

    private MoveManager moveManager;
    

    private void Awake()
    {
        moveManager = GetComponent<MoveManager>();
        stateMachine = GetComponent<StateMachine>();
        stateMachine.enabled = false;
    }

    private void Start()
    {
        EventCenter.Broadcast(EnumEventType.OnBattleUnitCreated,this);
    }

    public virtual Planet GetPlanet()
    {
        return this.ownerPlanet;
    }

    public void Init(Planet planet)
    {
        this.ownerPlanet = planet;
        
        moveManager.Init(planet);
        stateMachine.enabled = true;
    }
}
