using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackAble
{
    GameEntity GetAttackerOwner();
    GameEntity GetAttackEntity();
    void Attack();

    void OnSlainOther(GameEntity victim);

    void OnAttackOther(IVictimAble victimAble,AttackInfo attackInfo);
}
