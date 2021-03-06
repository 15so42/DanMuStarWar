using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以被设置为攻击目标
/// </summary>
public interface IDamageAble
{
    GameEntity GetVictimOwner();
    GameEntity GetVictimEntity();

    GameObject GetGameObject();

    void OnDamageOther(IVictimAble victimAble, BattleUnitProps.HpAndShield realDamage);

}
