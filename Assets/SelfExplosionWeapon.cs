using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfExplosionWeapon : HandWeapon
{
    public override void FireAnim()
    {
        animator.SetTrigger("Explosion");

        var quickExplosion = GetWeaponLevelByNbt("速爆");
        Invoke(nameof(Explosion),3-(quickExplosion*0.2f)<0?0:3-(quickExplosion*0.2f));
    }

    public override AttackInfo OnBeforeAttacked(AttackInfo attackInfo)
    {
        if (attackInfo.attackType == AttackType.Fire)
        {
            FireAnim();
        }
        return base.OnBeforeAttacked(attackInfo);
    }

    void Explosion()
    {
        var highExplosion=GetWeaponLevelByNbt("高爆");
        var attackInfo = new AttackInfo(owner, AttackType.Physics, 5+highExplosion*2);
        var position = transform.position;
        AttackManager.Instance.Explosion(attackInfo,this, position, 15,"MCExplosionFx");
        owner.Die();
    }
}
