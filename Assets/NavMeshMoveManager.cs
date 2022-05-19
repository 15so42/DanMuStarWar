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

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        rigidbody = GetComponent<Rigidbody>();
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

    public virtual void PushBack(Vector3 dir,float value=1)
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
        {
            StartCoroutine(PushBackC(dir, value));
        }
        
    }

    //存储上一次的重力大小，如果当前velocity.y
    private float lastVelocityY;
    IEnumerator PushBackC(Vector3 dir, float value)
    {
        rigidbody.velocity=dir * value;
        lastVelocityY = rigidbody.velocity.y;
        navMeshAgent.updatePosition = false;
        rigidbody.isKinematic = false;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            // RaycastHit raycastHit=new RaycastHit();
            // if (Physics.Raycast(transform.position, -1 * Vector3.up, out raycastHit, 0.2f))
            // {
            //     navMeshAgent.nextPosition = transform.position;
            //     navMeshAgent.updatePosition = true;
            //     rigidbody.isKinematic = true;
            //     Debug.Log("落地");
            //     yield break;
            // }
            
            if (lastVelocityY<0 && Mathf.Abs(rigidbody.velocity.y) < onGroundThreshold)
            {
                navMeshAgent.nextPosition = transform.position;
                navMeshAgent.updatePosition = true;
                rigidbody.isKinematic = true;
                //Debug.Log("落地");
                yield break;
            }

            lastVelocityY = rigidbody.velocity.y;
        }
        
        
    }
}
