using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonWeapon : HandWeapon
{
    
    
    public override void FireAnim()
    {
        var distance =
            Vector3.SqrMagnitude(owner.chaseTarget.GetVictimEntity().transform.position - owner.transform.position);

        if (distance < 100)
        {
            animator.SetTrigger("Attack");
           
            
            //Debug.LogError("Attack");
        }
        else
        {
            animator.SetTrigger("DragonBreath");
            //Debug.LogError("DragonBreath");
            
        }
        
    }

    
}
