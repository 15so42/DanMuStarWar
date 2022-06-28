using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DG.Tweening;
using UnityEngine;

public class FishingRodBullet : ArrowBullet
{
    private LineRenderer lineRenderer;
    [Header("鱼钩发射位置")]
    [HideInInspector]public Transform fishHook;


    public float backHookTime = 2;
    private float timer;//鱼钩两秒没击中目标，直接返回
    private Tween backHookSequence = null;
    //private RecycleAbleObject recycleAbleObject;
    private void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
       
    }

    public override void OnDisable()
    {
        base.OnDisable();
        timer = 0;
    }
    
    

    

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPositions(new Vector3[]{fishHook.transform.position,transform.position});
        timer += Time.deltaTime;
        if (timer > backHookTime)
        {
            //StartBackSequence();
            rigidbody.velocity = Vector3.zero;
            var dir = fishHook.transform.position - transform.position;
            transform.position+=(dir.normalized * (speed*0.02f * Time.deltaTime));
            if (dir.magnitude < 2f||timer>3.5f)
            {
                (handWeapon as BoomerangeWeapon).back = true;
                recycleAbleObject.Recycle();
            }
        }
    }

    public override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        timer = backHookTime;
    }

    // void StartBackSequence()
    // {
    //     timer = 0;
    //     backHookSequence?.Kill();
    //     backHookSequence=transform.DOMove( 1f).OnComplete(() =>
    //     {
    //         (handWeapon as BoomerangeWeapon).back = true;
    //         recycleAbleObject.Recycle();
    //     });
    // }
}
