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

    public void Explosion(AttackInfo attackInfo,Vector3 center,float radius)
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
            
        }
        ResFactory.Instance.CreateFx(GameConst.FX_PACMAN_EXPLOSION, center);

       
    }
}
