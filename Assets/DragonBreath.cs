using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBreath : MonoBehaviour
{
    public McUnit owner;
    public HandWeapon weapon;

    public LayerMask layerMask;
    
    private void OnTriggerEnter(Collider other)
    {
        var victimAble = other.GetComponentInChildren<IVictimAble>();
        
        //忽略和FETrigger的碰撞。
        if (victimAble != null  )
        {
            weapon.DamageOther(victimAble,new AttackInfo(owner,AttackType.Real,Mathf.CeilToInt(victimAble.GetVictimEntity().props.hp*0.01f)));
            //Debug.Log(other.gameObject.name + "|||"+other.gameObject.layer);
        }
    }
}
