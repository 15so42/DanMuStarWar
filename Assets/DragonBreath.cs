using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBreath : MonoBehaviour
{
    public McUnit owner;
    public HandWeapon weapon;


    private bool enable = true;
   

    private void OnEnable()
    {
        StartCoroutine(SwitchDamage());
    }

    private void Update()
    {
        // timer += Time.deltaTime;
        // if (timer > 0.1f)
        // {
        //     enable = false;
        //     if (timer > 0.3f)
        //     {
        //         timer = 0;
        //         enable = true;
        //     }
        // }
    }


    IEnumerator SwitchDamage()
    {
        while (true)
        {
            enable = false;
            yield return new WaitForSeconds(0.2f);
            enable = true;
            yield return null;
        }
    }
    
    
    private void OnTriggerStay(Collider other)
    {
        if (!enable) return;
        
        var victimAble = other.GetComponentInChildren<IVictimAble>();
        
        
        if (victimAble != null  )
        {
            weapon.pushBackHeight = 4;
            weapon.pushBackStrength = 3;
            weapon.DamageOther(victimAble,new AttackInfo(owner,AttackType.Real,Mathf.CeilToInt(victimAble.GetVictimEntity().props.hp*0.005f)));
            //Debug.Log(other.gameObject.name + "|||"+other.gameObject.layer);
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
