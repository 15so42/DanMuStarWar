using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackAble
{
    GameEntity GetAttackerOwner();
    GameEntity GetAttackEntity();
    void Attack();
}
