using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitProps : MonoBehaviour
{
    public int maxHp=100;
    public int hp=100;

    public int maxShield = 100;
    public int shield = 0;

    public int pDef = 50;//物理防御
    public int mDef = 50;//魔法防御

    public GameEntity gameEntity;
    public void Init(GameEntity gameEntity)
    {
        this.gameEntity = gameEntity;
        
    }

    public struct HpAndShield
    {
        public int hpValue;
        public int shieldValue;

        public HpAndShield(int hpValue, int shieldValue)
        {
            this.hpValue = hpValue;
            this.shieldValue = shieldValue;
        }

        
    }

    public void AddMaxHp(int value)
    {
        maxHp += value;
        hp += value;
    }
    
    public HpAndShield OnAttacked(AttackInfo attackInfo)
    {
        var damage = 1;
        if (attackInfo.attackType == AttackType.Physics)
        {
            damage = attackInfo.value * (1 - pDef / (100 + pDef));
        }

        if (attackInfo.attackType == AttackType.Magic)
        {
            damage = attackInfo.value * (1 - mDef / (100 + mDef));
        }

        if (attackInfo.attackType == AttackType.Real)
        {
            damage = attackInfo.value;
        }
        
        if (attackInfo.attackType == AttackType.Heal)
        {
            damage = -1 * attackInfo.value;
        }


        if (shield > 0)
        {
            if (damage >= shield)
            {
                shield = 0;
                damage -= shield;//伤害被护盾抵消一部分
            }

            if (damage < shield)
            {
                shield = shield - damage;
                damage = 0;//伤害被完全抵消
            }
                
            
        }
        
        hp -= damage;
        
        if (hp > maxHp)
            hp = maxHp;

        return new HpAndShield(hp,shield);
       
        
    }

    public bool IsAlive()
    {
        return hp > 0;
    }
}
