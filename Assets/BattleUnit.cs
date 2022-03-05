using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;
[IncludeInSettings(true)]
public class BattleUnit : MonoBehaviour
{
   
    public Planet ownerPlanet;

    private StateMachine stateMachine;

    private MoveManager moveManager;

    public Action<int,int> onHpChanged;

    public BattleUnitProps props;

    private HpBar hpUI;

    public GameManager gameManager;
    public BattleUnitManager battleUnitManager;
    public PlanetManager planetManager;

    public float findEnemyDistance = 7;
    private void Awake()
    {
        moveManager = GetComponent<MoveManager>();
        stateMachine = GetComponent<StateMachine>();
        //stateMachine.enabled = false;
        
        gameManager=GameManager.Instance;
        planetManager = gameManager.planetManager;
        battleUnitManager = gameManager.battleUnitManager;
    }

    public void OnAttacked(AttackInfo attackInfo)
    {
        var hpValue = props.OnAttacked(attackInfo);
        OnHpChanged(hpValue,props.maxHp);
    }

    private void Start()
    {
        EventCenter.Broadcast(EnumEventType.OnBattleUnitCreated,this);
        
        //UI
        hpUI = GameManager.Instance.uiManager.CreateHpBar(this);
        hpUI.Init(this);
    }

    public BattleUnit FindNearEnemy()
    {
        BattleUnit enemy = null;
        foreach (var unit in battleUnitManager.allBattleUnits)
        {
            if (Vector3.Distance(unit.transform.position, transform.position) < findEnemyDistance)
            {
                if (ownerPlanet.enemyPlayers.Contains(unit.ownerPlanet.owner))//敌对状态
                {
                    enemy = unit;
                    break;
                }
               
            }
        }

        return enemy;
    }

    public void OnHpChanged(int hp,int maxHP)
    {
        onHpChanged.Invoke(hp,maxHP);
    }

    public virtual Planet GetPlanet()
    {
        return this.ownerPlanet;
    }

    public void Init(Planet planet)
    {
        this.ownerPlanet = planet;
        
        moveManager.Init(planet);
        stateMachine.enabled = true;
    }
}
