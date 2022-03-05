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
    public float lerpValue = 10;

    [Header("guard")] public Planet planet;
    public GameObject guardPos;
    public float guardHeight = 5;

    public bool autoRotate = false;

    
    public void Init(Planet planet)
    {
        this.planet = planet;
      
    }

   

    public void SetTmpTarget(Vector3 tmpTarget)
    {
        this.tmpTarget = tmpTarget;
    }

    public void SetFinalTarget(Vector3 finalTarget)
    {
        this.finalTarget = finalTarget;
        this.tmpTarget = finalTarget;
        //Debug.Log("SetFinalTarget:"+finalTarget);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * (curSpeed * Time.deltaTime),Space.Self);
        if (autoRotate)//巡航时不启用方向控制
        {
            transform.forward=Vector3.Lerp(transform.forward,tmpTarget-transform.position,lerpValue*Time.deltaTime);
        }
        
    }

    
}
