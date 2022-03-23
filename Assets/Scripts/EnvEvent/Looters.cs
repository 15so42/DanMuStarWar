using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCode.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "GameEnvEvent/Looters", fileName = "Looters", order = 0)]
public class Looters : GameEnvEvent
{
   
    public override void Run()
    {
        var lootersCount = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < lootersCount; i++)
        {
            var battleUnit = ResFactory.Instance.CreateBattleUnit(GameConst.BATTLE_UNIT_LOOTER,
                UnityEngine.Random.insideUnitSphere.normalized * 300f);
        }
    }

   
}
