using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonStaffWeapon : HandWeapon
{
    [Header("召唤法杖攻击特效")] public GameObject summonWandAttack;

    public override void DamageOtherFx(IVictimAble victimAble)
    {
        base.DamageOtherFx(victimAble);
        SummonWandAttackFx(victimAble.GetVictimEntity().transform.position);
    }

    public void SummonWandAttackFx(Vector3 pos)
    {
        var fxGo=GameObject.Instantiate(summonWandAttack);
        fxGo.transform.position = pos;
        Destroy(fxGo,5);
    }

    public void Gather(Vector3 worldPos)
    {
        for (int i = 0; i < summons.Count; i++)
        {
            if (summons[i] == null || summons[i].IsAlive() == false)
            {
                summons.RemoveAt(i);
                i--;
                continue;
            }
                    
            summons[i].GoMCWorldPos(worldPos,false);
        }
    }
}
