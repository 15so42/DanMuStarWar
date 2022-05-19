using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TntWeapon : BowWeapon
{
    
    
  
    public override void InitArrow(ArrowBullet arrowComp,Vector3 dir)
    {
        var bowStrength = GetWeaponLevelByNbt("黏性");
        (arrowComp as TntBullet).Init(owner, dir);

        (arrowComp as TntBullet).SetSticky(true);
    }
    
}
