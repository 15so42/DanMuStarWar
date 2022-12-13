using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class McTowerWeapon : HandWeapon
{
    public GameObject linePfb;
    public Transform shootPoint;
    public string animStr = "Attack";
    public float fxDelay = 0;
    
    public override void FireAnim()
    {
        animator.SetTrigger(animStr);
        
       
        Invoke(nameof(Fx),fxDelay);
        Invoke(nameof(DamageTargetByReal),fxDelay);
    }

    protected virtual void Fx()
    {
        var start = shootPoint==null?transform.position:shootPoint.transform.position;
        var end = owner.chaseTarget.GetVictimEntity().GetVictimPosition();
        var line = LineRenderManager.Instance.SetLineRender(start,end,linePfb
        );
        line.SetPosition(0,start);
        for (int i = 1; i < 4; i++)
        {
            line.SetPosition(i,GetRandomPos(start,end));
        }
        line.SetPosition(4,end);
        Destroy(line.gameObject,0.6f);
    }

    public void DamageTargetByReal()
    {
        DamageOther(owner.chaseTarget,new AttackInfo(owner,AttackType.Real,Mathf.CeilToInt(owner.chaseTarget.GetVictimEntity().props.maxHp*0.07f)));
    }
    
    
    public override AttackInfo GetBaseAttackInfo()
    {
        return new AttackInfo(owner,AttackType.Real,attackValue);
    }

    Vector3 GetRandomPos(Vector3 start,Vector3 end)
    {
        return new Vector3(UnityEngine.Random.Range(start.x,end.x),UnityEngine.Random.Range(start.y,end.y),UnityEngine.Random.Range(start.z,end.z) );
    }
}
