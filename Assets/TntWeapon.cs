using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TntWeapon : BowWeapon
{
    
    
  
    public override void InitArrow(ArrowBullet arrowComp,Vector3 dir)
    {
        //var sticky = GetWeaponLevelByNbt("黏性")>0;
        (arrowComp as TntBullet).Init(owner, dir,this);

        var highExplosive=GetWeaponLevelByNbt("高爆");
        (arrowComp as TntBullet).SetSticky(true);
        (arrowComp as TntBullet).SetHighExplosive(highExplosive);
        (arrowComp as TntBullet).SetFollowTarget(owner.chaseTarget);
        
        
    }
    
}
