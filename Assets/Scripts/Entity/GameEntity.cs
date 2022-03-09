using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(BattleUnitProps))]
[RequireComponent(typeof(SkillContainer))]
public abstract class GameEntity : MonoBehaviour
{
   private StateMachine stateMachine;
   
   public Action<int,int> onHpChanged;

   public BattleUnitProps props;
   public SkillContainer skillContainer;

   public HpBar hpUI;
   [Header("hpUIOffse")] public Vector3 hpUIOffse;
   [Header("HPUIScale")] public Vector3 hpUIScale;

   public void Awake()
   {
      stateMachine = GetComponent<StateMachine>();
      props = GetComponent<BattleUnitProps>();
      skillContainer = GetComponent<SkillContainer>();
      
      skillContainer.Init(this);
   }
   
   public void Start()
   {
      //UI
      hpUI = GameManager.Instance.uiManager.CreateHpBar(this);
      hpUI.Init(this);
      stateMachine.enabled = true;
   }

   public abstract void LogTip(string tip);
   
   public void OnAttacked(AttackInfo attackInfo)
   {
      var hpValue = props.OnAttacked(attackInfo);
      OnHpChanged(hpValue,props.maxHp);
   }
   
   public void OnHpChanged(int hp,int maxHP)
   {
      onHpChanged.Invoke(hp,maxHP);
   }

   public virtual void AddSkill(SkillItemUI skillItemUi)
   {
   
      
      skillContainer.AddSkill(skillItemUi);
       
   
   }

}
