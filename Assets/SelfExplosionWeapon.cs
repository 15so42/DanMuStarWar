using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfExplosionWeapon : HandWeapon
{
    [Header("自爆羊直接爆炸")]
    public bool explosionSheep = false;
    public override void FireAnim()
    {
        animator.SetTrigger("Explosion");

        var quickExplosion = GetWeaponLevelByNbt("速爆");
        var cd = 3 - (quickExplosion * 0.2f) < 0 ? 0 : 3 - (quickExplosion * 0.2f);
        if (explosionSheep)
            cd = 0;
        Invoke(nameof(Explosion),cd);
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
        if(owner.die)
            return;
        var highExplosion=GetWeaponLevelByNbt("高爆");
        var value = 5 + highExplosion * 2;
        if (explosionSheep)
        {
            value = 250;
        }
        var attackInfo = new AttackInfo(owner, AttackType.Physics, value);
        var position = transform.position;
        
        AttackManager.Instance.Explosion(attackInfo,this, position, explosionSheep?15:10,"MCExplosionFx");
        owner.Die();
    }
}
