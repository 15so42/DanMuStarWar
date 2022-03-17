using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        

    }

    protected void Start()
    {
       base.Start();
       hpUI.SetColor(ownerPlanet.planetColor);
      
       
       EventCenter.Broadcast(EnumEventType.OnBattleUnitCreated,this);
        //SkillManager.Instance.AddSkill("Skill_腐蚀_LV1",this);
    }

    public GameEntity FindNearEnemy(bool onlyEnemyPlanet)
    {
        GameEntity enemy = null;
        var enemyPlanets = ownerPlanet.enemyPlanets;
        if (!onlyEnemyPlanet)
        {
            enemyPlanets = PlanetManager.Instance.allPlanets;
        }
        foreach (var planet in enemyPlanets)
        {
            foreach (var enemyUnit in planet.battleUnits)
            {
                if(Random.Range(0,planet.battleUnits.Count)>0)
                    continue;
                if (Vector3.Distance(enemyUnit.transform.position, transform.position) < findEnemyDistance)
                {
                   
                        enemy = enemyUnit;
                        break;
                    
               
                }
            }

            if (Random.Range(0, enemyPlanets.Count) > 0)
            {
                continue;
            }

            enemy = planet;
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

    public override void Die()
    {
        base.Die();
        stateMachine.enabled = false;
        ownerPlanet.battleUnits.Remove(this);
        DieFx();
        //Destroy(gameObject);
        
        Destroy(hpUI.gameObject);
        
        gameObject.SetActive(false);
        
    }
}
