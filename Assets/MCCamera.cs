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
    public float speed=1;
    
    List<BattleUnit> battleUnits=new List<BattleUnit>();

    [Header("更换目标时间")] public int minChangeCd=3;
    public int maxChangeCd=7;
    public int tmpCd;
    
    //DoTween
    private Tween  moveSequence;
    
    // Start is called before the first frame update
    void Start()
    {
        //camera = GetComponent<Camera>();
        battleUnits = BattleUnitManager.Instance.allBattleUnits;

        tmpCd = UnityEngine.Random.Range(minChangeCd, maxChangeCd);
    }

    Vector3 curVelocity;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > tmpCd || target && target.die)
        {
            if(battleUnits.Count==0)
                return;
            target = battleUnits[UnityEngine.Random.Range(0, battleUnits.Count)];
            
            // if(target==null || target.die)
            //     return;
            // Vector3 targetPos=target.transform.position + offset;
            //
            //
            // moveSequence?.Kill();
            // moveSequence = transform.DOMove(targetPos, 2f).SetEase(Ease.InOutCubic);
            
            timer = 0;
            tmpCd = UnityEngine.Random.Range(minChangeCd, maxChangeCd);
        }

        if(target==null || target.die)
            return;
        Vector3 targetPos=target.transform.position + offset;
        var distance = targetPos.magnitude;
        Vector3 dir = (targetPos - transform.position).normalized;
        if (distance > 0)
        {
            //transform.Translate(dir * (speed * Time.deltaTime));
            
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref curVelocity, 3);
        }
        
        //transform.position = transform.position + dir * (speed * Time.deltaTime); 
        
    }
}
