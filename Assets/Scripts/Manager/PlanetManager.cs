using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public List<Planet> allPlanets=new List<Planet>();

    private void Awake()
    {
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetCreated,OnPlanetCreated);
    }

    void OnPlanetCreated(Planet planet)
    {
        allPlanets.Add(planet);
    }
}
