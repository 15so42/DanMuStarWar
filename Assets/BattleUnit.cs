using System;
using System.Collections;
using System.Collections.Generic;
using Ludiq;
using UnityEngine;
[IncludeInSettings(true)]
public class BattleUnit : MonoBehaviour
{
   
    public Planet planet;

    

    private MoveManager moveManager;
    private StateController stateController;

    private void Awake()
    {
        moveManager = GetComponent<MoveManager>();
        stateController = GetComponent<StateController>();
    }

    public Planet GetPlanet()
    {
        return this.planet;
    }

    public void Init(Planet planet)
    {
        this.planet = planet;
        stateController.Init(planet);
        stateController.SetupAI(true,null);
        moveManager.Init(planet);
    }
}
