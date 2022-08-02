using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderCloud : MonoBehaviour
{
    public IAttackAble owner;

    public float moveSpeed = 3;
    private float timer = 0;

    public float attackCd = 1;
    // Start is called before the first frame update

    private int lifeTime=10;
    private AttackInfo attackInfo;
    private int count=1;
    
    
    public void Init(IAttackAble owner, AttackInfo attackInfo, float cd, int lifeTime,int count)
    {
        this.owner = owner;
        this.attackInfo = attackInfo;
        this.attackCd = cd;
        this.lifeTime = lifeTime;
        this.count = count;
        Destroy(gameObject,lifeTime);
    }
   
    
    Vector3 dir=Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > attackCd)
        {
            Attack();
            dir = UnityEngine.Random.insideUnitSphere;
            timer = 0;
        }

        dir.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, dir, 0.01f * Time.deltaTime);
        transform.Translate(dir * (moveSpeed * Time.deltaTime),Space.Self);
    }

    void Attack()
    {
        var transform1 = transform;
        var position = transform1.position;
        AttackManager.Instance.Thunder(owner,attackInfo,null,position,3,position-Vector3.up*15,15,count);
    }
}
