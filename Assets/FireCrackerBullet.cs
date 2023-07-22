using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCrackerBullet : ArrowBullet
{

    private int highExplosiveLevel = 0;
    

    protected override void DamageVictim(IVictimAble victim)
    {
        //base.DamageVictim(victim);
        ExplosionDamage();
    }

    void ExplosionDamage() 
    {
            
        var attackInfo = new AttackInfo(owner, AttackType.Physics, Mathf.CeilToInt(3+highExplosiveLevel*1f));
        var position = transform.position;
        AttackManager.Instance.Explosion(attackInfo,handWeapon, position, 12,"MCExplosionFx");
        //Debug.Log(gameObject.name+"Explosion一次");

        var rand = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < rand; i++)
        {
            var circle = UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0, 25);
            var pos = position + new Vector3(circle.x, 0, circle.y);
            AttackManager.Instance.Explosion(attackInfo,handWeapon, pos, 12,"MCExplosionFx");
        }
        recycleAbleObject.Recycle();
        
    }

  
    public void SetHighExplosive(int level)
    {
        this.highExplosiveLevel = level;
    }
}
