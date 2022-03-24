﻿using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;
using Random = UnityEngine.Random;

[IncludeInSettings(true)]
public class BattleUnit : GameEntity,IAttackAble,IVictimAble
{
   
    public Planet ownerPlanet;

    private MoveManager moveManager;

    public GameManager gameManager;
    
    [HideInInspector] public BattleUnitManager battleUnitManager;
    [HideInInspector] public PlanetManager planetManager;

    public float findEnemyDistance = 7;
    public IVictimAble chaseTarget = null;

    
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
        var target = chaseTarget?.GetVictimEntity();
        return chaseTarget != null && target.IsAlive() && Vector3.Distance(target.transform.position,transform.position)<findEnemyDistance;
    }
   

 

    protected void Start()
    {
       base.Start();
       if (showHpUI)
       {
           if (ownerPlanet)
           {
               hpUI.SetColor(ownerPlanet.planetColor);
           }
           else
           {
               hpUI.SetColor(Color.white);
           }
       }
       
       
      
       
       EventCenter.Broadcast(EnumEventType.OnBattleUnitCreated,this);
        //SkillManager.Instance.AddSkill("Skill_腐蚀_LV1",this);
    }

    public virtual GameEntity FindNearEnemy(bool onlyEnemyPlanet)
    {
        GameEntity enemy = null;
        var enemyPlanets = ownerPlanet.enemyPlanets;
        if (!onlyEnemyPlanet)
        {
            enemyPlanets = PlanetManager.Instance.allPlanets;
        }

        if (enemyPlanets.Count <= 0)
            return null;
        var planet = enemyPlanets[Random.Range(0, enemyPlanets.Count)];
        if(planet)
        {
            if (planet.battleUnits.Count == 0)
            {
                enemy = planet;
            }
            else
            {
                var enemyUnit = planet.battleUnits[Random.Range(0, planet.battleUnits.Count)];
                if (Vector3.Distance(enemyUnit.transform.position, transform.position) < findEnemyDistance)
                {
                   
                    enemy = enemyUnit;
                
                }
            }

        }
        
            
        return enemy;
    }

    public void SetChaseTarget(IVictimAble target)
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

        if (showHpUI)
        {
            Destroy(hpUI.gameObject);
        }
        
        
        gameObject.SetActive(false);
        
    }
    
    public override void OnAttacked(AttackInfo attackInfo)
    {
        base.OnAttacked(attackInfo);
        if (!IsTargetAlive())//当自己处于和平状态时被袭击
        {
            if(Math.Abs(supportDistance) < 0.5f)
                return;
            if (attackInfo.attacker.GetAttackerOwner() == GetVictimOwner())
            {
                return;
                
            }
            for (int i = 0; i < ownerPlanet.battleUnits.Count; i++)
            {
                if (Vector3.Distance(ownerPlanet.battleUnits[i].transform.position, transform.position) < supportDistance)
                {
                    var supportAble = ownerPlanet.battleUnits[i].GetComponent<ISupportAble>();
                    if (supportAble != null)
                    {
                        supportAble.Support(attackInfo.attacker as BattleUnit);
                    }
                }
            }
        }
       
    }


    public override GameEntity GetAttackerOwner()
    {
        return ownerPlanet;
    }

    public override GameEntity GetVictimOwner()
    {
        return ownerPlanet;
    }

   
}
