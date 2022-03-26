using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum CameraStatus
{
    Normal,
    CloseUp,
    Shake,
}

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    private Camera cam;
    public Vector3 center;
    public Vector3 offset;

    public CameraStatus cameraStatus;

   
    private GameObject closeUpObj;
    private float closeUpDistance = 10;
    private float closeUpTimer=0;
    void Start()
    {
        cam = GetComponent<Camera>();
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetCreated,OnPlanetCreated);
        EventCenter.AddListener(EnumEventType.OnPlanetsSpawned,OnPlanetsSpawned);
    }

    public void BeginAnim()
    {
        transform.transform.DOMove(center+offset,3f).SetEase(Ease.OutQuart);
    }

    void OnPlanetCreated(Planet planet)
    {
        
    }

    void OnPlanetsSpawned()
    {
        BeginAnim();
    }

    public void ShakeCamera()
    {
        cameraStatus = CameraStatus.Shake;
        transform.DOShakePosition(1, 3).OnComplete(() =>
        {
            cameraStatus = CameraStatus.Normal;
        });
    }
    public void StartCloseUpObj(GameObject go,float distance)
    {
        closeUpObj = go;
       
        closeUpDistance = distance;
        cameraStatus = CameraStatus.CloseUp;
    }

    public void StopCloseUp()
    {
        closeUpTimer = 0;
        cameraStatus = CameraStatus.Normal;
    }
 
    void LateUpdate()
    {
        
        if (cameraStatus==CameraStatus.CloseUp)
        {
            closeUpTimer += Time.deltaTime;
            if (closeUpTimer > 5)
            {
                StopCloseUp();
            }
            transform.LookAt(closeUpObj.transform.position);
            //transform.RotateAround(closeUpObj.transform.position,10*Time.deltaTime);
            transform.position = Vector3.Slerp( transform.position,closeUpObj.transform.position + closeUpObj.transform.forward * closeUpDistance,1f*Time.deltaTime);
            
        }
        else if (cameraStatus == CameraStatus.Shake)
        {
            //DoNothing
        }
        else
        {
            transform.position = center + offset;
            transform.LookAt(center);
        }
       
    }

    
   

  
}