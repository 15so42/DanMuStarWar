using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType{
    Physics,
    Magic,
    Real
}
public class AttackInfo
{
    public GameEntity attacker;
    public AttackType attackType;
    public int value;


    public AttackInfo(GameEntity attacker, AttackType attackType, int value)
    {
        this.attacker = attacker;
        this.attackType = attackType;
        this.value = value;
    }
}
