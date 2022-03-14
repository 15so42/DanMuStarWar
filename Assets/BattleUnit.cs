using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;

[IncludeInSettings(true)]
public class BattleUnit : GameEntity
{
   
    public Planet ownerPlanet;

    private MoveManager moveManager;

    public GameManager gameManager;
    
    [HideInInspector] public BattleUnitManager battleUnitManager;
    [HideInInspector] public PlanetManager planetManager;

    public float findEnemyDistance = 7;
    public GameEntity chaseTarget = null;
    protected void Awake()
    {
        base.Awake();
        moveManager = GetComponent<MoveManager>();
       
        
        
        gameManager=GameManager.Instance;
        planetManager = gameManager.planetManager;
        battleUnitManager = gameManager.battleUnitManager;
    }

    public bool IsTargetAlive()
    {
        return chaseTarget != null && chaseTarget.IsAlive() && Vector3.Distance(chaseTarget.transform.position,transform.position)<findEnemyDistance;
    }
   

    protected void OnEnable()
    {
        
        EventCenter.Broadcast(EnumEventType.OnBattleUnitCreated,this);

    }

    protected void Start()
    {
       base.Start();
       hpUI.SetColor(ownerPlanet.planetColor);
       //SkillManager.Instance.AddSkill("Skill_腐蚀_LV1",this);
    }

    public BattleUnit FindNearEnemy()
    {
        BattleUnit enemy = null;
        foreach (var planet in ownerPlanet.enemyPlanets)
        {
            foreach (var enemyUnit in planet.battleUnits)
            {
                if (Vector3.Distance(enemyUnit.transform.position, transform.position) < findEnemyDistance)
                {
                   
                        enemy = enemyUnit;
                        break;
                    
               
                }
            }
        }
       
        
        return enemy;
    }

    public void SetChaseTarget(BattleUnit target)
    {
        this.chaseTarget = target;
    }
    

    public void Init(Planet planet)
    {
        this.ownerPlanet = planet;
        moveManager.Init(planet);
        
    }

    public override void LogTip(string tip)
    {
        Debug.Log(tip);
    }
}
