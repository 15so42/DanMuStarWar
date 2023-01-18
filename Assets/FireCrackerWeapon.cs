using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCrackerWeapon : BowWeapon
{
    public override void InitArrow(ArrowBullet arrowComp,Vector3 dir)
    {
        //var sticky = GetWeaponLevelByNbt("黏性")>0;
        (arrowComp as FireCrackerBullet).Init(owner, dir,this);

        var highExplosive=GetWeaponLevelByNbt("高爆");
        
        (arrowComp as FireCrackerBullet).SetHighExplosive(highExplosive);
        
        
        
    }
    
   
}
