using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCode.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "GameEnvEvent/SolarStorm", fileName = "SolarStorm", order = 0)]
public class SolarStorm : GameEnvEvent
{
    public float xSize = 300;
    public float ySize = 50;
    public override void Run()
    {
        var battleUnits = BattleUnitManager.Instance.allBattleUnits;
        var mainCamera=Camera.main.GetComponent<MultipleTargetCamera>();
        mainCamera.ShakeCamera();
        
        //mainCamera.GetComponent<CameraShake>().Play();
        ResFactory.Instance.CreateFx(GameConst.FX_SOLAR_STORM, Vector3.zero);
        /*for (int i = 0; i < battleUnits.Count; i++)
        {
            var change = Random.Range(0, 2) > 0;
            if (battleUnits[i] != null && battleUnits[i].die == false)
            {
                battleUnits[i].OnAttacked(new AttackInfo(null,AttackType.Magic, (int)(battleUnits[i].props.hp*0.5) ));
            }
        }*/
        FightingManager.Instance.StartCoroutine(Storm());
    }

    IEnumerator Storm()
    {
        var count = 0;
        while (true)
        {
            if(count>900)
                yield break;
            count++;
            Vector3 RandomPoint=new Vector3(UnityEngine.Random.Range(-xSize,xSize),UnityEngine.Random.Range(-ySize,ySize),-200f);
           
            RaycastHit hitInfo=new RaycastHit();

            if (Physics.Raycast(RandomPoint, Vector3.forward, out hitInfo, 2000))
            {
                var victim = hitInfo.collider.GetComponent<IVictimAble>();
                if (victim!=null)
                {
                    victim.OnAttacked(new AttackInfo(null,AttackType.Magic,Random.Range(10,60)));
                }

                var bullet = ResFactory.Instance.CreateBullet(GameConst.BULLET_SOLAR_STORM, RandomPoint).GetComponent<Bullet>();
                bullet.SetEndPos(hitInfo.point);
                GameObject fx = ResFactory.Instance.CreateFx(GameConst.FX_BULLET_HIT, hitInfo.point);
                
            }
            else
            {
                var bullet = ResFactory.Instance.CreateBullet(GameConst.BULLET_SOLAR_STORM, RandomPoint).GetComponent<Bullet>();
                bullet.SetEndPos(RandomPoint+Vector3.forward*1000);
            }
            yield return null;
        }
    }
}
