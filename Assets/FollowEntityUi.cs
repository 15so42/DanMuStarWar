using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEntityUi : MonoBehaviour
{
    protected GameObject owner;
    public Vector3 offset=Vector3.up;
    private Camera mainCamera;


    public void Init(GameObject target)
    {
        owner = target;
    }
    
    private void Awake()
    {
        mainCamera=Camera.main;
    }
    
    private void LateUpdate()
    {
        UpDatePos();
    }

    void UpDatePos()
    {
        if(owner && mainCamera)
            transform.position = mainCamera.WorldToScreenPoint(owner.transform.position)+offset;
    }
}
