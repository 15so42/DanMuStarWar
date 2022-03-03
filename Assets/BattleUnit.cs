using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    private Planet planet;

    private MoveManager moveManager;
    private StateController stateController;

    private void Awake()
    {
        moveManager = GetComponent<MoveManager>();
        stateController = GetComponent<StateController>();
    }

    public void Init(Planet planet)
    {
        this.planet = planet;
        stateController.Init(planet);
        stateController.SetupAI(true,null);
        moveManager.Init(planet);
    }
}
