using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;
 
    public Vector3 offset;
    public float smoothTime = .5f;
 
    public float maxZoom = 40f;
    public float minZoom = 10f;
    public float zoomLimiter = 50f;
 
    private Vector3 velocity;
    private Camera cam;

    public bool moveAble = false;

    public Bounds bounds = new Bounds();
    void Start()
    {
        cam = GetComponent<Camera>();
    }
 
    void LateUpdate()
    {
        if (targets.Count == 0)
            return;
 
        if(moveAble)
            Move();
        Zoom();
        transform.LookAt(bounds.center);
    }

    public void AddTarget(Transform transform)
    {
        targets.Add(transform);
    }

    public void ClearTargets()
    {
        targets.Clear();
    }
    
    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }
 
    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        transform.parent.transform.position = centerPoint;
 
        //Vector3 newPosition = centerPoint + offset;
 
        //transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
 
    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
                i--;
            }

            bounds.Encapsulate(targets[i].position);
        }
 
        return bounds.size.x*1.4f;
    }
 
    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }
 
        bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
 
        return bounds.center;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireCube(bounds.center,bounds.size);
    }
}