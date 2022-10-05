using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NavMeshMoveManager : MoveManager
{

    private NavMeshAgent navMeshAgent;

    private Rigidbody rigidbody;
    public float onGroundThreshold = 0.3f;

    private McUnit steve;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        rigidbody = GetComponent<Rigidbody>();
        steve = GetComponent<McUnit>();
    }

    public override void StartBoost(float value)
    {
        navMeshAgent.speed += value;
    }

    public override void EndBoost(float value)
    {
        navMeshAgent.speed -= value;
    }

    public override void SetTmpTarget(Vector3 tmpTarget)
    {
        base.SetTmpTarget(tmpTarget);
        navMeshAgent.SetDestination(tmpTarget);
    }

    protected override void Update()
    {
        //DoNothing
    }

    public void PushBackByPos(Vector3 victimPos,Vector3 attackerPos,float upForce,float pushForce,float multiplier=1)
    {
        if(steve.canPushBack==false)
            return;

        
        var angryLevel=steve.GetActiveWeapon().GetWeaponLevelByNbt("愤怒");
        var toughLevel=steve.GetActiveWeapon().GetWeaponLevelByNbt("坚韧");
        
        if (angryLevel > 0)
        {
            multiplier -= 0.05f * angryLevel;
        }
        if (toughLevel > 0)
        {
            multiplier -= 0.05f * toughLevel;
        }
        
        Vector3 retDir=Vector3.zero;
        Vector3 horDir = (victimPos - attackerPos).normalized;
        horDir.y = 0;
        retDir = (horDir * pushForce) + upForce*Vector3.up;
        PushBack(retDir,multiplier);
    }

    public virtual void PushBack(Vector3 dir,float multiplier=1)
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
        {
            StartCoroutine(PushBackC(dir, multiplier));
        }
        
    }

    //存储上一次的重力大小，如果当前velocity.y
    private float lastVelocityY;
    IEnumerator PushBackC(Vector3 dir, float multiplier)
    {
        rigidbody.velocity=dir * multiplier;
        lastVelocityY = rigidbody.velocity.y;
        navMeshAgent.updatePosition = false;
        rigidbody.isKinematic = false;

        var timer=0f;//落地或者一定时间后强行落地

        while (true)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
            
            RaycastHit raycastHit=new RaycastHit();
            Debug.DrawLine(transform.position+Vector3.up*0.2f,transform.position-transform.up*0.5f);
            if (timer>3 || rigidbody.velocity.magnitude<1  /*||lastVelocityY<=0 && Physics.Raycast(transform.position+Vector3.up*0.2f, -1 * Vector3.up, out raycastHit, 0.5f/*,~LayerMask.GetMask("BattleUnit"))*/)
            {
                navMeshAgent.nextPosition = transform.position;
                navMeshAgent.updatePosition = true;
                rigidbody.isKinematic = true;
                //Debug.Log("落地");
                yield break;
            }
            
            // if (lastVelocityY<0 && Mathf.Abs(rigidbody.velocity.y) < onGroundThreshold)
            // {
            //     navMeshAgent.nextPosition = transform.position;
            //     navMeshAgent.updatePosition = true;
            //     rigidbody.isKinematic = true;
            //     //Debug.Log("落地");
            //     yield break;
            // }

            lastVelocityY = rigidbody.velocity.y;
        }
        
        
    }

    public override void Stop()
    {
        base.Stop();
        navMeshAgent.isStopped = true;
    }

    public override void Enable()
    {
        base.Enable();
        navMeshAgent.isStopped = false;
    }
}
