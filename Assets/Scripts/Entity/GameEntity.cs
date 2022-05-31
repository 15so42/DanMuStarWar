using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using GameCode.Tools;
using UnityEngine;


public enum GameEntityType
{
   BattleUnit,
   Planet,
}
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(BattleUnitProps))]
[RequireComponent(typeof(SkillContainer))]
public abstract class GameEntity : MonoBehaviour,IAttackAble,IVictimAble
{
   protected StateMachine stateMachine;
   
   public Action<int,int,int,int> onHpChanged;

   [HideInInspector]public BattleUnitProps props;
   [HideInInspector]public SkillContainer skillContainer;

   //public GameEntityType gameEntityType = GameEntityType.BattleUnit;
   [Header("手动设置半径")] public float radius=5;
   [Header("呼叫友军来支援的距离")] public float supportDistance=30;

   public bool die = false;


   public bool showHpUI=true;
   public HpBar hpUI;

   
   [Header("hpUIOffse")] public Vector3 hpUIOffse;
   [Header("HPUIScale")] public Vector3 hpUIScale=Vector3.one;
   
   //击杀管理
   protected IAttackAble lastAttacker;//最后一击
   
   //击杀相关事件
   public Func<AttackInfo,AttackInfo> onBeforeAttacked;
   public Action<IVictimAble,int> onAttackOther;
   public Action<AttackInfo> onAttacked;
   public Action onSlainOther;

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

   public virtual void AddShield(int value)
   {
      props.shield += value;
      if (props.shield > props.maxShield)
         props.shield = props.maxShield;
      onHpChanged.Invoke(props.hp,props.maxHp,props.shield,props.maxShield);
   }

   public GameObject GetGameObject()
   {
      return gameObject;
   }

   public virtual AttackInfo OnBeforeAttacked(AttackInfo attackInfo)
   {
      if (onBeforeAttacked != null)
      {
         attackInfo=onBeforeAttacked?.Invoke(attackInfo);
      }
      
      return attackInfo;
   }

   public virtual BattleUnitProps.HpAndShield OnAttacked(AttackInfo attackInfo)
   {
      
      
      attackInfo=OnBeforeAttacked(attackInfo);//减伤增伤判断
      
      var hpAndShield = props.OnAttacked(attackInfo);//返回剩余生命值和护盾和实际伤害
      
      OnHpChanged(hpAndShield.hpValue,props.maxHp,hpAndShield.shieldValue,props.maxShield);
      
      if (attackInfo.attackType != AttackType.Heal)
      {
         attackInfo.attacker?.OnAttackOther(this, attackInfo);
         FlyText.Instance.ShowDamageText(transform.position+Vector3.up,attackInfo.value+"");
      }
      else
      {
         FlyText.Instance.ShowHealText(transform.position+Vector3.up,attackInfo.value+"");
      }
      
      //事件触发

      if(attackInfo.attackType!=AttackType.Heal && (GameEntity) attackInfo.attacker!=this)
         onAttacked?.Invoke(attackInfo);
     
      if (hpAndShield.hpValue <= 0 && !die)
      {
         lastAttacker = attackInfo.attacker;
         Die();
      }

      
      return hpAndShield;

   }
   
   public void OnHpChanged(int hp,int maxHP,int shield,int maxShield)
   {
      onHpChanged?.Invoke(hp,maxHP,shield,maxShield);
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

   public virtual void OnSlainOther()
   {
      //throw new NotImplementedException();
      onSlainOther?.Invoke();
   }

   public virtual void OnAttackOther(IVictimAble victimAble, AttackInfo attackInfo)
   {
      //throw new NotImplementedException();
      if (attackInfo.attackType != AttackType.Heal && (GameEntity) victimAble!=this)
      {
         onAttackOther?.Invoke(victimAble,attackInfo.value);
      }
        
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
