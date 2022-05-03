using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;

public class LineWeapon : Weapon
{
    public GameObject lineRendererPfb;


    public override bool FireCheck()
    {
        if (owner.chaseTarget != null && !owner.chaseTarget.GetVictimEntity().die)
            return true;
        return false;
    }

    public override void Fire()
    {
        var victim = owner.chaseTarget.GetVictimEntity();
        victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,attackValue));
        
        
        var endPos = victim.transform.position;
        var line= LineRenderManager.Instance.SetLineRender(transform.position, endPos, lineRendererPfb);
        
        GameObject fx = ResFactory.Instance.CreateFx(GameConst.FX_BULLET_HIT,endPos );

    }

   
}
