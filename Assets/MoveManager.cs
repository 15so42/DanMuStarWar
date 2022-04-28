using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveManager : MonoBehaviour
{
    public float curSpeed = 10;

    public float maxSpeed = 10;

    public Vector3 finalTarget;

    public Vector3 tmpTarget;
    public float rotateSpeed = 0.5f;

    [Header("guard")] public Planet planet;
    public GameObject guardPos;
    public float guardHeight = 5;

    public bool autoRotate = false;

    
    public void Init(Planet planet)
    {
        this.planet = planet;
      
    }
    
    

   

    public virtual void SetTmpTarget(Vector3 tmpTarget)
    {
        this.tmpTarget = tmpTarget;
    }

    public virtual void SetFinalTarget(Vector3 finalTarget,bool syncTmpTarget)
    {
        this.finalTarget = finalTarget;
            if(syncTmpTarget)
                this.tmpTarget = finalTarget;
            
        //Debug.Log("SetFinalTarget:"+finalTarget);
    }

    protected virtual void Update()
    {
        transform.Translate(Vector3.forward * (curSpeed * Time.deltaTime),Space.Self);
        if (autoRotate)//巡航时不启用方向控制
        {
            //transform.forward=Vector3.Lerp(transform.forward,tmpTarget-transform.position,lerpValue*Time.deltaTime);
            
            // Determine which direction to rotate towards
            Vector3 targetDirection = tmpTarget - transform.position;

            // The step size is equal to speed times frame time.
            float singleStep = rotateSpeed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        if (Vector3.Distance(transform.position, tmpTarget) < 1)
        {
            SetTmpTarget(finalTarget);
        }
        
    }

    
}
