using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonWeapon : HandWeapon
{
    public override void FireAnim()
    {
        animator.SetTrigger("DragonBreath");
        
        
    }
}
