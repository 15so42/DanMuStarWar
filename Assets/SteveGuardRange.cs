using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteveGuardRange : MonoBehaviour
{
    private McUnit owner;
    public ParticleSystem fx;
    public void Init(McUnit owner,float radius)
    {
        this.owner = owner;
        owner.dieAction += OnOwnerDie;
        var targetScale=Vector3.one*radius;
        targetScale.y = 1;
        transform.localScale = targetScale;
        
        var shapeModule = fx.shape;
        shapeModule.scale = new Vector3(radius,radius,1);
    }

    private void OnTriggerEnter(Collider other)
    {
        var victim=owner.EnemyCheck(other);
        if (victim != null && owner.IsTargetAlive() == false)
        {
            owner.SetChaseTarget(victim);
            Debug.Log("驻守领域设置新敌人"+victim.GetGameObject().name);
        }
    }

    void OnOwnerDie()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        owner.dieAction -= OnOwnerDie;
    }
}
