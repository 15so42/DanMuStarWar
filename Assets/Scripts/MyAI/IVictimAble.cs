using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以被设置为攻击目标
/// </summary>
public interface IVictimAble
{
    GameEntity GetVictimOwner();
    GameEntity GetVictimEntity();

    void OnAttacked(AttackInfo attackInfo);

}
