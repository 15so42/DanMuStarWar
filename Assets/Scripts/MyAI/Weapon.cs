using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
public class Weapon : MonoBehaviour
{
    public float attackDistance;
    public BattleUnit owner;
    
    
    //武器攻速计时
    public float attackSpeed = 1;
    private float timer = 0;
    private bool ready = true;
    //散射角度，当检测方式为ray时，射击向量在玩家到敌人的方向进行对应度数的随机取值以模拟误差射击效果，如若射中玩家或者障碍物，发射子弹到对应位置并附带拖尾
    [Header("散射角度")]
    public float scatteringAngel=5;

    public float rayDistance = 20;

    [Header("伤害配置")] public AttackType attackType;
    
    public int attackValue = 1;
    public GameObject lineRendererPfb;
    public void Init(BattleUnit owner)
    {
        this.owner = owner;

        
        timer = 1 / attackSpeed;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            ready = true;
            
            
        }
    }

    public void Attack()
    {
        if (ready)
        {
            Fire();
            ready = false;
            timer=1 / attackSpeed;
        }
    }
    public void Fire()
    {
        var dir = owner.chaseTarget.transform.position - transform.position;
        var distance = dir.sqrMagnitude;
        if ( distance< attackDistance)
        {
          
            //散射算法
            Vector3 newVec = Quaternion.Euler(Random.Range(-scatteringAngel,scatteringAngel),Random.Range(-scatteringAngel,scatteringAngel),0)*dir;
            //PhotonView.Get(this).RPC("FireBullet",RpcTarget.All,newVec);
                
            RaycastHit hitInfo=new RaycastHit();
            
            if (Physics.Raycast(transform.position, newVec,out hitInfo, rayDistance))
            {
                var prop = hitInfo.collider.GetComponent<BattleUnitProps>();
                if (prop)
                {
                    prop.gameEntity.OnAttacked(new AttackInfo(attackType,attackValue));
                }
                //lineRenderer.SetPositions(new Vector3[]{transform.position,hitInfo.point});
                LineRenderManager.Instance.SetLineRender(transform.position, hitInfo.point, lineRendererPfb);
            }
            else
            {
                LineRenderManager.Instance.SetLineRender(transform.position, transform.position+dir*rayDistance, lineRendererPfb);
                //lineRenderer.SetPositions(new Vector3[]{transform.position,transform.position+dir*rayDistance});
            }
        }
    }
}
