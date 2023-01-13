using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;
using Random = System.Random;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance;
    [Header("落雷")] public GameObject thunderPfb;
    [Header("雨裁")] public GameObject rainAttack;
    [Header("空间斩")] public GameObject spaceChopperPfb;
    [Header("召唤法杖攻击特效")] public GameObject summonWandAttack;

    private void Awake()
    {
        Instance = this;
    }

    //落泪
    public void Thunder(IAttackAble attacker,AttackInfo damage,HandWeapon weapon,Vector3 startPoint,float randomRadius,Vector3 targetPos,float findradius,int count)
    {
        List<IVictimAble> enemys = GetEnemyInRadius(attacker, targetPos, findradius, count);
        for (int i = 0; i < enemys.Count; i++)
        {
            var circle = UnityEngine.Random.insideUnitCircle * randomRadius;
            Vector3 randomPos = startPoint + new Vector3(circle.x, 0, circle.y);
            //var go = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
            //go.transform.position = hit.point;
            //Destroy(go,2);
            Vector3 end = enemys[i].GetVictimPosition();

            ThunderFx(randomPos, enemys[i].GetVictimPosition(), 1 + Vector3.Distance(startPoint, end) / 60);


            //Debug.LogError("这不有吗！！！");
            //FlyText.Instance.ShowDamageText(end - Vector3.up * 3, "落雷(" + damage.value + ")");
            
            var hpAndShield=enemys[i].OnAttacked(damage);
            // if(weapon!=null)
            //     weapon.OnDamageOther(enemys[i],hpAndShield);
            
        }
          
        
    }
    
    protected virtual void ThunderFx(Vector3 start,Vector3 end,float width=1)
    {
        var line = LineRenderManager.Instance.SetLineRender(start,end,thunderPfb
        );
        line.positionCount = 5;
        // line.startWidth = width;
        // line.endWidth = width;
        line.SetPosition(0,start);
        for (int i = 1; i < 4; i++)
        {
            line.SetPosition(i,GetRandomPos(start,end));
        }
        line.SetPosition(4,end);
        Destroy(line.gameObject,0.6f);
    }
    Vector3 GetRandomPos(Vector3 start,Vector3 end)
    {
        return new Vector3(UnityEngine.Random.Range(start.x,end.x),UnityEngine.Random.Range(start.y,end.y),UnityEngine.Random.Range(start.z,end.z) );
    }


    public List<IVictimAble> GetVictimsInRadius(Vector3 center, float radius)
    {
        List<IVictimAble> victimAbles=new List<IVictimAble>();
        var colliders = Physics.OverlapSphere(center,radius);
        foreach (var collider in colliders)
        {
            var mcUnit = collider.GetComponent<McUnit>();
            if (mcUnit!=null)
            {
                victimAbles.Add(mcUnit);
            }
        }
        return victimAbles;
        
    }
    public List<IVictimAble> GetEnemyInRadius(IAttackAble able,Vector3 center, float radius,int needCount)
    {
        List<IVictimAble> victimAbles=new List<IVictimAble>();
        var colliders = Physics.OverlapSphere(center,radius);
        foreach (var collider in colliders)
        {
            var victim = (able as McUnit).EnemyCheck(collider);
            if((able as McUnit).EnemyCheck(collider)!=null)
                victimAbles.Add(victim);
            if(victimAbles.Count>=needCount)
                break;
        }

        return victimAbles;
    }

    
    //碰撞体范围碰撞后得到的单位传入，返回敌人
    protected List<IVictimAble> FilterEnemies(List<IVictimAble> victimAbles,IAttackAble attacker)
    {
        List<IVictimAble> result=new List<IVictimAble>();
        foreach (var e in victimAbles)
        {
            var victim = (attacker as McUnit).EnemyCheck(e);
            if(victim!=null)
                result.Add(victim);
            
        }

        return result;
    }


    public void RainAttackFx(Vector3 pos)
    {
        var rainAttackGo=GameObject.Instantiate(rainAttack);
        rainAttackGo.transform.position = pos;
    }

    public void SpaceChopperFx(Vector3 pos)
    {
        var spaceGo=GameObject.Instantiate(spaceChopperPfb);
        spaceGo.transform.position = pos;
    }

   

    public void AttackEnemies(List<IVictimAble> enemies,AttackInfo attackInfo)
    {
        foreach (var e in enemies)
        {
            e.OnAttacked(attackInfo);
        }
    }

    public void Explosion(AttackInfo attackInfo,IDamageAble damageAble,Vector3 center,float radius,string fxName="")
    {
        var attacker = attackInfo.attacker;
        var colliders = Physics.OverlapSphere(center, radius);
        foreach (var collider in colliders)
        {
            var gameEntity = collider.GetComponent<GameEntity>();
            if (!gameEntity)//不是单位
                continue;

            var gameEntityOwner = gameEntity.GetVictimOwner();
            if (gameEntity==null || gameEntityOwner == attacker.GetAttackerOwner()) //同星球
                continue;
            if (gameEntity.die)//已经死亡
                continue;
            
            var hpAndShield = gameEntity.OnAttacked(attackInfo);
            damageAble?.OnDamageOther(gameEntity,hpAndShield);



            //steve
            var navMove = gameEntity.GetComponent<NavMeshMoveManager>();
            if (navMove)
            {
                navMove.PushBackByPos( gameEntity.transform.position,transform.position,3,2,1);
            }
            
        }

        if (fxName == "")
            fxName = GameConst.FX_PACMAN_EXPLOSION;
        ResFactory.Instance.CreateFx(fxName, center);

       
    }
    
    
    public int GetMaxValueFromHES(IVictimAble victimAble)
    {
        var victimEntity = victimAble.GetVictimEntity();
        if (victimEntity)
        {
            var prop = victimEntity.props;
            var maxHp = prop.maxHp;
            var maxShied = prop.maxShield;
            var maxEndurance = (victimEntity as McUnit).GetActiveWeapon().maxEndurance;
            return Mathf.Max(new int[] {maxEndurance, maxHp, maxShied});
        }

        return 200;

    }
    
    public int GetAvgValueFromHES(IVictimAble victimAble)
    {
        var victimEntity = victimAble.GetVictimEntity();
        if (victimEntity)
        {
            var count = 3;
            var prop = victimEntity.props;
            var maxHp = prop.maxHp;
            var maxShied = prop.maxShield;
            var maxEndurance = 0;
            var mcUnit = victimAble as McUnit;
            if (mcUnit!=null)
            {
                var weapon = mcUnit.GetActiveWeapon();
                maxEndurance = weapon == null ? 100 : mcUnit.GetActiveWeapon().maxEndurance;
            }
            else
            {
                count--;
            }

            
            return (maxEndurance+maxHp+maxShied)/count;
        }

        return 200;

    }
}
