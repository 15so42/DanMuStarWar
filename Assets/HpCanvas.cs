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
    public Transform commanderUiParent1;
    [Header("Mc模式commanderUi")] public GameObject mcCommanderUiPfb;
    
    
    public GameObject ringUiPfb;
    public Transform ringUiParent;

    [Header("集结UI")] public GameObject gatherUiPfb;
    public Transform gatherUiParent;

    [Header("McMarkUi")] public McPosMarkUi mcMarkUiPfb;
    public Transform mcMarkUiParent;

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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="area">0左1右</param>
    /// <returns></returns>
    public CommanderUI CreateCommanderUi(GameObject gameObject,int area=0)
    {
        return  GameObject.Instantiate(commanderUiPfb,area==0?commanderUiParent:commanderUiParent1).GetComponent<CommanderUI>();
        
    }
    
    public CommanderUI CreateSteveCommanderUi(GameObject gameObject,int area=0)
    {
        return  GameObject.Instantiate(mcCommanderUiPfb,area==0?commanderUiParent:commanderUiParent1).GetComponent<CommanderUI>();
        
    }
    
    // public  CreateCommanderUi(GameObject gameObject,int area=0)
    // {
    //     return  GameObject.Instantiate(commanderUiPfb,area==0?commanderUiParent:commanderUiParent1).GetComponent<CommanderUI>();
    //     
    // }
    
    public GatherUI CreateGatherUi()
    {
        return  GameObject.Instantiate(gatherUiPfb,gatherUiParent).GetComponent<GatherUI>();
        
    }

    public McPosMarkUi CreateMcMarkUi()
    {
        return GameObject.Instantiate(mcMarkUiPfb,mcMarkUiParent).GetComponent<McPosMarkUi>();
    }
    
}
