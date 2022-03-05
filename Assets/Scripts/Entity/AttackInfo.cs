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
    public AttackType attackType;
    public int value;


    public AttackInfo(AttackType attackType, int value)
    {
        this.attackType = attackType;
        this.value = value;
    }
}
