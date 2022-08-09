﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType{
    Physics,
    Magic,
    Real,
    Heal,
    Reflect,
    Fire,
    Poison,
    Thunder,
    
    
}
public class AttackInfo
{
    public IAttackAble attacker;
    public AttackType attackType;
    public int value;
    public Color color=Color.white;


    public AttackInfo(IAttackAble attacker, AttackType attackType, int value,Color color)
    {
        this.attacker = attacker;
        this.attackType = attackType;
        this.value = value;
        this.color = color;
    }
    public AttackInfo(IAttackAble attacker, AttackType attackType, int value)
    {
        this.attacker = attacker;
        this.attackType = attackType;
        this.value = value;
    }
}
