using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.Rendering;

public class Steve : WarPlane
{
    public int ownerPosIndex;

    public Vector3 targetMcPos = Vector3.zero;

    private FightingManager fightingManager;

    public SkinnedMeshRenderer[] meshRenderers;

    protected override void Start()
    {
        base.Start();
       
        fightingManager = GameManager.Instance.fightingManager;
        moveManager = GetComponent<MoveManager>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public Vector3 GetPos(int index)
    {
        return fightingManager.mcPosManager.GetPosByIndex(index);
    }

    public override void GoMCPos(Vector3 pos)
    {
        base.GoMCPos(pos);

        targetMcPos = pos;
        CustomEvent.Trigger(gameObject, "OnDestinationSet");
    }

    public bool NearTargetPos()
    {
        float distance = Vector3.Distance(transform.position, moveManager.tmpTarget);
        return distance < 1f;
    }

    public override IVictimAble EnemyCheck(Collider collider)
    {
        var gameEntity = collider.GetComponent<GameEntity>();
        if (!gameEntity)//不是单位
            return null;

        var gameEntityOwner = gameEntity.GetVictimOwner();
        if (gameEntity==null || gameEntityOwner == GetAttackerOwner()) //同星球
            return null;
        if (gameEntity.die)//已经死亡
            return null;

        var targetPlanet = gameEntityOwner as Planet;
        if(targetPlanet==null )//如果只对敌对星球寻敌，而敌对星球不存在，或找到的单位不属于，不算作敌人
            return null;
            
        if (ownerPlanet.enemyPlanets.Contains(targetPlanet))
        {
            return gameEntity;
        }

        return null;


    }
    
    private void Update()
    {
        //throw new NotImplementedException();
    }
    

    /// <summary>
    /// 受击特效
    /// </summary>
    /// <param name="attackInfo"></param>
    public override void OnAttacked(AttackInfo attackInfo)
    {
        base.OnAttacked(attackInfo);
        StopAllCoroutines();
        if (gameObject.activeSelf)
        {
            StartCoroutine(VictimFx());
        }
        

    }

   

    IEnumerator VictimFx()
    {
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.EnableKeyword("_EMISSION");
            meshRenderers[i].material.SetColor("_EmissionColor",new Color(1,0,0));
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetColor("_EmissionColor",new Color(0,0,0));
        }
    }
}
