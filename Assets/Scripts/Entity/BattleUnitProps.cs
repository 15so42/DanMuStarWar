using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitProps : MonoBehaviour
{
    public int maxHp=100;
    public int hp=100;

    public int pDef = 50;//物理防御
    public int mDef = 50;//魔法防御

    public int OnAttacked(AttackInfo attackInfo)
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

        hp -= damage;
        return hp;
    }
}
