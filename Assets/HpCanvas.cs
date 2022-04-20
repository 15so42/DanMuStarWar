using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCanvas : MonoBehaviour
{
    public GameObject hpBarPfb;
    public Transform hpBarParent;

    [Header("PlanetUI")]
    public GameObject planetUiPfb;
    public Transform planetUiParent;

    [Header("CommanderUI")] public GameObject commanderUiPfb;
    public Transform commanderUiParent;
    
    public GameObject ringUiPfb;
    public Transform ringUiParent;

    [Header("SkillUI")] public GameObject skillUIPfb;
    public Transform skillUIParent;
    public HpBar CreateHpBar(GameEntity gameEntity)
    {
        return  GameObject.Instantiate(hpBarPfb, hpBarParent).GetComponent<HpBar>();
        
    }
    
    public ColonyRingUi CreateRingUi(Planet planet)
    {
        return  GameObject.Instantiate(ringUiPfb, ringUiParent).GetComponent<ColonyRingUi>();
        
    }
    
    public PlanetUI CreatePlanetUI(Planet planet)
    {
        return  GameObject.Instantiate(planetUiPfb, planetUiParent).GetComponent<PlanetUI>();
        
    }
    
    public CommanderUI CreateCommanderUi(GameObject gameObject)
    {
        return  GameObject.Instantiate(commanderUiPfb, commanderUiParent).GetComponent<CommanderUI>();
        
    }
    
    
}
