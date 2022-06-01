using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType{
    Physics,
    Magic,
    Real,
    Heal,
    Reflect
}
public class AttackInfo
{
    public IAttackAble attacker;
    public AttackType attackType;
    public int value;


    public AttackInfo(IAttackAble attacker, AttackType attackType, int value)
    {
        this.attacker = attacker;
        this.attackType = attackType;
        this.value = value;
    }
}
