using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class McTowerWeapon : HandWeapon
{
    public GameObject linePfb;
    public override void FireAnim()
    {
        animator.SetTrigger("Attack");
        
        Invoke(nameof(Damage),0f);
        var start = transform.position;
        var end = owner.chaseTarget.GetVictimEntity().GetVictimPosition();
        var line = LineRenderManager.Instance.SetLineRender(start,end,linePfb
            );
        line.SetPosition(0,transform.position);
        for (int i = 1; i < 4; i++)
        {
            line.SetPosition(i,GetRandomPos(start,end));
        }
        line.SetPosition(4,end);
        Destroy(line.gameObject,0.6f);
    }

    Vector3 GetRandomPos(Vector3 start,Vector3 end)
    {
        return new Vector3(UnityEngine.Random.Range(start.x,end.x),UnityEngine.Random.Range(start.y,end.y),UnityEngine.Random.Range(start.z,end.z) );
    }
}
