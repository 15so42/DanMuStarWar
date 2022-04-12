using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCode.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "GameEnvEvent/Looters", fileName = "Looters", order = 0)]
public class Looters : GameEnvEvent
{
   
    public override void Run(int level)
    {
        List<GameObject> looterList=new List<GameObject>();
        var camera = Camera.main.GetComponent<MultipleTargetCamera>();
        var center =camera.center;
        var lootersCount = UnityEngine.Random.Range(1, 3);
        for (int i = 0; i < lootersCount; i++)
        {
            var pos = center+UnityEngine.Random.insideUnitSphere.normalized * 200;
            //pos.y = UnityEngine.Random.Range(-20, 20);
            var battleUnit = ResFactory.Instance.CreateBattleUnit(GameConst.BATTLE_UNIT_LOOTER,
                pos);
            battleUnit.transform.LookAt(center);
            looterList.Add(battleUnit);
        }
        
        camera.StartCloseUpObj(looterList[UnityEngine.Random.Range(0,looterList.Count)],30f);
    }

   
}
