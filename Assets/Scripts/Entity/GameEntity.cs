using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using GameCode.Tools;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(BattleUnitProps))]
[RequireComponent(typeof(SkillContainer))]
public abstract class GameEntity : MonoBehaviour,IAttackAble,IVictimAble
{
   protected StateMachine stateMachine;
   
   public Action<int,int> onHpChanged;

   public BattleUnitProps props;
   public SkillContainer skillContainer;
   [Header("手动设置半径")] public float radius=5;
   [Header("呼叫友军来支援的距离")] public float supportDistance=30;

   public bool die = false;


   public bool showHpUI=true;
   public HpBar hpUI;

   
   [Header("hpUIOffse")] public Vector3 hpUIOffse;
   [Header("HPUIScale")] public Vector3 hpUIScale;
   
   //击杀管理
   protected IAttackAble lastAttacker;//最后一击

   public void Awake()
   {
      stateMachine = GetComponent<StateMachine>();
      props = GetComponent<BattleUnitProps>();
      props.Init(this);
      skillContainer = GetComponent<SkillContainer>();
      
      skillContainer.Init(this);
      EventCenter.AddListener(EnumEventType.OnStartWaitingJoin,OnStartWaitingJoin);
   }
   
   public void Start()
   {
      //UI
      if (showHpUI)
      {
         hpUI = GameManager.Instance.uiManager.CreateHpBar(this);
         hpUI.Init(this);
      }

      stateMachine.enabled = true;
   }

   public bool IsAlive()
   {
      return  die==false;
   }
   

   public abstract void LogTip(string tip);
   
   public virtual void OnAttacked(AttackInfo attackInfo)
   {
      var hpValue = props.OnAttacked(attackInfo);
      OnHpChanged(hpValue,props.maxHp);
      

      if (hpValue <= 0 && !die)
      {
         lastAttacker = attackInfo.attacker;
         Die();
      }
   }
   
   public void OnHpChanged(int hp,int maxHP)
   {
      onHpChanged?.Invoke(hp,maxHP);
   }

   public virtual void AddSkill(SkillItemUI skillItemUi)
   {
   
      
      skillContainer.AddSkill(skillItemUi);
       
   
   }

   public virtual bool AddSkillCheck(string skillName)
   {
      return skillContainer.AddSkillCheck(skillName);
   }

   public virtual void DieFx()
   {
      
   }
   public virtual void Die()
   {
      if(die)
         return;
      die = true;
      ResFactory.Instance.CreateFx(GameConst.FX_BULLET_HIT, transform.position);
      //Destroy(gameObject);不销毁，销毁可能导致各种引用丢失的问题
   }

   private void OnDestroy()
   {
      EventCenter.RemoveListener(EnumEventType.OnStartWaitingJoin,OnStartWaitingJoin);
      
   }

   public virtual void OnStartWaitingJoin()
   {
      if (!die)
      {
         Die();
      }

      if (gameObject)
      {
         Destroy(gameObject);
      }
   }

   public virtual GameEntity GetAttackerOwner()
   {
      return null;//需重写
   }

   public virtual GameEntity GetAttackEntity()
   {
      return this;
   }

   public virtual void Attack()
   {
     //Do nothing
   }
   
   

   public virtual GameEntity GetVictimOwner()
   {
      return null;//需重写
   }

   public virtual GameEntity GetVictimEntity()
   {
      return this;
   }
}
