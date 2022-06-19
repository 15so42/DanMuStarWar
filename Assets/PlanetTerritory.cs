using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTerritory : MonoBehaviour
{
    public Planet ownerPlanet;
    public List<BattleUnit> inTerritory=new List<BattleUnit>();

    private void Start()
    {
        ownerPlanet = GetComponent<Planet>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var entrant = other.GetComponent<BattleUnit>();
        if(entrant==null)
            return;
       
        foreach (var b in inTerritory)
        {
            if (b != null)
            {
                b.OnBattleUnitEnterPlanet(entrant, ownerPlanet);
            }
            
        }
        inTerritory.Add(entrant);
    }

    private void OnTriggerExit(Collider other)
    {
        var leaver = other.GetComponent<BattleUnit>();
        if(leaver==null)
            return;
        inTerritory.Remove(leaver);
    }
}
