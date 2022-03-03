﻿using System;
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
    public float guardRotateSpeed = 20;
    
    public void Init(Planet planet)
    {
        this.planet = planet;
        
        //生成球面导航点

        GameObject guardPosParent = new GameObject("guardPosParent");
        guardPosParent.transform.SetParent(planet.transform);
        guardPosParent.transform.localPosition=Vector3.zero;
        guardPosParent.AddComponent<RotateSelf>().rotateSpeed = guardRotateSpeed;
        
        guardPos = new GameObject("guardPos");
        guardPos.transform.SetParent(guardPosParent.transform);
        guardPos.transform.localPosition = guardPos.transform.position + Vector3.up * guardHeight;
        
        guardPosParent.transform.Rotate(Random.insideUnitSphere * Random.Range(0,360));
        
    }

    public void SetTmpTarget(Vector3 tmpTarget)
    {
        this.tmpTarget = tmpTarget;
    }

    public void SetFinalTarget(Vector3 finalTarget)
    {
        this.finalTarget = finalTarget;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * (curSpeed * Time.deltaTime),Space.Self);
        transform.forward=Vector3.Lerp(transform.forward,tmpTarget-transform.position,lerpValue*Time.deltaTime);
    }

    public void Guard()
    {
        var position = guardPos.transform.position;
        SetTmpTarget(position);
        SetFinalTarget(position);
    }
}
