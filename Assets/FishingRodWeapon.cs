using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodWeapon : BoomerangeWeapon
{
    public Transform fishHook;
    public override void InitArrow(ArrowBullet arrowComp,Vector3 dir)
    {
        var bowStrength = GetWeaponLevelByNbt("力量");
        arrowComp.Init(owner, dir,this);
        arrowComp.SetStrength(bowStrength);
        (arrowComp as FishingRodBullet).fishHook = fishHook;
    }

    
   
}
