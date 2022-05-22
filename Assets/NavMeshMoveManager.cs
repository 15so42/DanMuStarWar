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

    private Steve steve;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        rigidbody = GetComponent<Rigidbody>();
        steve = GetComponent<Steve>();
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

    public void PushBackByPos(Vector3 victimPos,Vector3 selfPos,float upForce,float pushForce,float multiplier=1)
    {
        if(steve.canPushBack==false)
            return;

        Vector3 retDir=Vector3.zero;
        Vector3 horDir = (victimPos - selfPos).normalized;
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
            if (timer>3 || lastVelocityY<0 && Physics.Raycast(transform.position, -1 * Vector3.up, out raycastHit, 0.5f))
            {
                navMeshAgent.nextPosition = transform.position;
                navMeshAgent.updatePosition = true;
                rigidbody.isKinematic = true;
                Debug.Log("落地");
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
}
