using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using GameCode.Tools;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(BattleUnitProps))]
[RequireComponent(typeof(SkillContainer))]
public abstract class GameEntity : MonoBehaviour
{
   protected StateMachine stateMachine;
   
   public Action<int,int> onHpChanged;

   public BattleUnitProps props;
   public SkillContainer skillContainer;
   [Header("手动设置半径")] public float radius=5;

   public bool die = false;
   

   public HpBar hpUI;
   [Header("hpUIOffse")] public Vector3 hpUIOffse;
   [Header("HPUIScale")] public Vector3 hpUIScale;

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
      hpUI = GameManager.Instance.uiManager.CreateHpBar(this);
      hpUI.Init(this);
      stateMachine.enabled = true;
   }

   public bool IsAlive()
   {
      return props.IsAlive();
   }
   

   public abstract void LogTip(string tip);
   
   public virtual void OnAttacked(AttackInfo attackInfo)
   {
      var hpValue = props.OnAttacked(attackInfo);
      OnHpChanged(hpValue,props.maxHp);
      if (hpValue <= 0)
      {
         Die();
      }
   }
   
   public void OnHpChanged(int hp,int maxHP)
   {
      onHpChanged.Invoke(hp,maxHP);
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
      ResFactory.Instance.CreateFx(GameConst.FX_BULLET_HIT, transform.position);
     //Destroy(gameObject);不销毁，销毁可能导致各种引用丢失的问题
   }

   public virtual void OnStartWaitingJoin()
   {
      //为新的流程做好准备
      Destroy(gameObject);
   }

}
