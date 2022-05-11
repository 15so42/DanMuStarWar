using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = Bolt.Sequence;

[Serializable]
public class MCCamera : MonoBehaviour
{
    //private Camera camera;
    private float timer;
    private GameEntity target;
    
    public Vector3 offset;

    public Vector3 initPos;

    [Header("更换目标时间")] public int minChangeCd=3;
    public int maxChangeCd=7;
    public int tmpCd;
    
    //DoTween
    private Tween  moveSequence;
    
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        timer = 0;
        tmpCd = UnityEngine.Random.Range(minChangeCd, maxChangeCd);
    }

    public void SetTarget(GameEntity gameEntity)
    {
        timer = 0;
        target = gameEntity;
    }

    Vector3 curVelocity;
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos=initPos;
        if (target)
        {
            timer += Time.deltaTime;
            targetPos=target.transform.position + offset;
            if (timer > tmpCd)
            {
                timer = 0;
                tmpCd = UnityEngine.Random.Range(minChangeCd, maxChangeCd);
                target = null;
                return;//进入下一帧
            }
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref curVelocity, 3);
        
    }
}
