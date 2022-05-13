using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DG.Tweening;
using UnityEngine;

public class Bullet : RecycleAbleObject
{
    public float speed = 100;

    public Sequence sequence;

    private Vector3 endPos;
    private TrailRenderer trailRenderer;
    public void SetEndPos(Vector3 pos)
    {
        this.endPos = pos;
        if (started)
        {
            trailRenderer.Clear();
            sequence = DOTween.Sequence();
            var distance = Vector3.Distance(endPos, transform.position);
            sequence.Append(transform.DOMove(endPos, distance / speed));
        }
    }

    public void Init()
    {
        
    }

    private bool started = false;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void OnEnable()
    {
        base.OnEnable();
        trailRenderer.Clear();
    }
    
    private void Start()
    {
        trailRenderer.Clear();
        sequence = DOTween.Sequence();
        var distance = Vector3.Distance(endPos, transform.position);
        sequence.Append(transform.DOMove(endPos, distance / speed));
        started = true;
    }

   

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.forward * (speed * Time.deltaTime),Space.Self);
    }

    public override void Recycle()
    {
        sequence.Kill();
        base.Recycle();
    }
}
