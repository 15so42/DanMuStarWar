using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class ThunderStaffWeapon : McTowerWeapon
{
    private float lastCloudTime = 0;
    private float lastLinkTime = 0;
    private float lastThundersTime = 0;//落雷
    
    //List<UnityTimer.Timer> timers=new List<Timer>();
    public GameObject cloudPfb;
    
    public override AttackInfo GetBaseAttackInfo()
    {
        return new AttackInfo(owner,AttackType.Physics,attackValue+(int)(GetWeaponLevelByNbt("高压")*1.5f));
    }

    protected override void Fx()
    {
        if(owner.chaseTarget==null)
            return;
        var linkLevel = GetWeaponLevelByNbt("闪电链");
        if (linkLevel > 0 && Time.time>lastLinkTime+6)
        {
            Vector3 mainTargetPos = owner.chaseTarget.GetVictimEntity().transform.position;
           
            var count = Mathf.CeilToInt(linkLevel/4f);
            float radius = 20+linkLevel;
            
                var list=AttackManager.Instance.GetEnemyInRadius(owner, mainTargetPos,
                    radius, count+1);
                list.Remove(owner.chaseTarget);
                
            

            FxOther(list,mainTargetPos);
            lastLinkTime = Time.time;

        }

        //落雷
        var thunderLevel = GetWeaponLevelByNbt("落雷");
        if (thunderLevel > 0 && Time.time>lastThundersTime+6)
        {
            float radius = 5+ thunderLevel ;
            var count = 1+thunderLevel / 4;
            var attackInfo = GetBaseAttackInfo();
            //var damage = new AttackInfo(attackInfo.attacker, attackInfo.attackType, attackInfo.value * 5);
            Vector3 targetPos = owner.chaseTarget.GetVictimEntity().GetVictimPosition();
            AttackManager.Instance.Thunder(owner,attackInfo,this,targetPos+Vector3.up*60,6,targetPos,radius,count);
            lastThundersTime = Time.time;
        }
        
        //雷云
        var cloudLevel = GetWeaponLevelByNbt("雷云");
        if (cloudLevel > 0)
        {
            var cd = 60- (60* ((float)cloudLevel/(cloudLevel+15)) );
            if (Time.time > lastCloudTime + cd)
            {
                var cloud = GameObject.Instantiate(cloudPfb).GetComponent<ThunderCloud>();
                cloud.transform.position = owner.chaseTarget.GetVictimPosition() + Vector3.up * 15;
                //固定攻速1，冷却缩减计算公式为（等级/等级+15），持续时间为10+等级,每次可攻击目标每10级增加一次
                cloud.Init(owner,GetBaseAttackInfo(),1.8f,10+cloudLevel,1+cloudLevel/10);
                lastCloudTime = Time.time;
            }
        }

        base.Fx();
    }
    
    
    

    protected virtual void FxOther(List<IVictimAble> targets,Vector3 mainTargetPos)
    {
        if(targets==null || targets.Count==0)
            return;
        var start =mainTargetPos;
        var end = targets[targets.Count-1].GetVictimEntity().GetVictimPosition();
        var line = LineRenderManager.Instance.SetLineRender(start,end,linePfb
        );
        
        line.positionCount = targets.Count+1;
        line.SetPosition(0,start);
        
        for (int i = 0; i < targets.Count ; i++)
        {
            var hpAndShield=targets[i].OnAttacked(GetBaseAttackInfo());
            OnDamageOther(targets[i],hpAndShield);
            targets[i].OnAttacked(GetBaseAttackInfo());
            line.SetPosition(i+1,targets[i].GetVictimEntity().GetVictimPosition());
        }

        
        
        Destroy(line.gameObject,0.6f);
    }
    
    
}