using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using UnityEngine;

public class TntBullet : ArrowBullet
{
    private Material material;

    public RecycleAbleObject recycleAbleObject;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        material.EnableKeyword("_EMISSION");
        
        recycleAbleObject = GetComponent<RecycleAbleObject>();
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(TNTExplosion());
    }


    private void OnCollisionEnter(Collision other)
    {
        //DoNothing
    }

    IEnumerator TNTExplosion()
    {
        yield return new WaitForSeconds(2);
        
        material.SetColor("_EmissionColor",new Color(1,0,0));
        yield return new WaitForSeconds(0.5f);
        material.SetColor("_EmissionColor",new Color(0,0,0));
        yield return new WaitForSeconds(1);
        
        material.SetColor("_EmissionColor",new Color(1,0,0));
        yield return new WaitForSeconds(0.5f);
        material.SetColor("_EmissionColor",new Color(0,0,0));
        yield return new WaitForSeconds(1);

        ExplosionDamage();
        //ExplosionFx();

    }

    void ExplosionDamage()
    {
        
            var attackInfo = new AttackInfo(owner, AttackType.Physics, 5);
            var position = transform.position;
            AttackManager.Instance.Explosion(attackInfo, position, 15,"MCExplosionFx");
            //Debug.Log(gameObject.name+"Explosion一次");
            recycleAbleObject.Recycle();
        
    }
}
