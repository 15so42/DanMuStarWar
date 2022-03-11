using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance;

    void Awake()
    {
        Instance = this;
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetCreated,OnPlanetCreated);
    }
    public List<Planet> allPlanets=new List<Planet>();

    public List<Planet> ownerAblePlanets=new List<Planet>();
   

    void OnPlanetCreated(Planet planet)
    {
        allPlanets.Add(planet);
        if (planet.canBeOwner)
        {
            ownerAblePlanets.Add(planet);
        }
    }
}
