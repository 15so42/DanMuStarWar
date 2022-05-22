using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Explosion(AttackInfo attackInfo,Vector3 center,float radius,string fxName="")
    {
        var attacker = attackInfo.attacker;
        var colliders = Physics.OverlapSphere(center, radius);
        foreach (var collider in colliders)
        {
            var gameEntity = collider.GetComponent<GameEntity>();
            if (!gameEntity)//不是单位
                continue;

            var gameEntityOwner = gameEntity.GetVictimOwner();
            if (gameEntity==null || gameEntityOwner == attacker.GetAttackerOwner()) //同星球
                continue;
            if (gameEntity.die)//已经死亡
                continue;
            
            gameEntity.OnAttacked(attackInfo);
            
            //steve
            var navMove = gameEntity.GetComponent<NavMeshMoveManager>();
            if (navMove)
            {
                navMove.PushBackByPos( gameEntity.transform.position,transform.position,3,2,1);
            }
            
        }

        if (fxName == "")
            fxName = GameConst.FX_PACMAN_EXPLOSION;
        ResFactory.Instance.CreateFx(fxName, center);

       
    }
}
