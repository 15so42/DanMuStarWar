﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCanvas : MonoBehaviour
{
    public GameObject hpBarPfb;
    public Transform hpBarParent;

    [Header("PlanetUI")]
    public GameObject planetUiPfb;
    public Transform planetUiParent;

    [Header("SkillUI")] public GameObject skillUIPfb;
    public Transform skillUIParent;
    public HpBar CreateHpBar(GameEntity gameEntity)
    {
        return  GameObject.Instantiate(hpBarPfb, hpBarParent).GetComponent<HpBar>();
        
    }
    public PlanetUI CreatePlanetUI(Planet planet)
    {
        return  GameObject.Instantiate(planetUiPfb, planetUiParent).GetComponent<PlanetUI>();
        
    }
    
    
}
