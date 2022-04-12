using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCode.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "GameEnvEvent/SolarStorm", fileName = "SolarStorm", order = 0)]
public class SolarStorm : GameEnvEvent
{
    
    public override void Run(int level)
    {
        
        var mainCamera=Camera.main.GetComponent<MultipleTargetCamera>();
        mainCamera.ShakeCamera();
        
        
        ResFactory.Instance.CreateFx(GameConst.FX_SOLAR_STORM, Vector3.zero);
      
        FightingManager.Instance.StartCoroutine(Storm(level));
    }

    IEnumerator Storm(int level)
    {
        var count = 0;
        while (true)
        {
            if(count>15)
                yield break;
            count++;
            

            for (int i = 0; i < PlanetManager.Instance.allPlanets.Count; i++)
            {
                var planet = PlanetManager.Instance.allPlanets[i];
                planet.OnAttacked(new AttackInfo(null,AttackType.Magic,Random.Range(5,10)+level * 2));
                GameObject fx = ResFactory.Instance.CreateFx(GameConst.FX_BULLET_HIT, planet.transform.position);
            }
            
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
