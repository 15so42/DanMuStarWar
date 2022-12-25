using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class ChristmasTree : MonoBehaviour
{
    public GameObject giftBox;

    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,15*60);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 45)
        {
            var units = AttackManager.Instance.GetVictimsInRadius(transform.position, 25f);
            foreach (var unit in units)
            {
                Parabola(transform.position+Vector3.up*10,unit.GetVictimPosition(),unit);
            }
            Debug.Log("发礼物咯！！");
            timer = 0;
        }
    }

    public void Parabola(Vector3 start, Vector3 end,IVictimAble mcUnit)
    {
        GameObject giftBoxGo = Instantiate(giftBox, transform.position + Vector3.up * 10, Quaternion.identity);
        DOTween.To(setter: value =>
            {
                //Debug.Log(value);
                giftBoxGo.transform.position = EMath.Parabola(start, end, 10, value);
            }, startValue: 0, endValue: 1, duration: 2)
            .SetEase(Ease.Linear).OnComplete(() =>
            {
                mcUnit.OnAttacked(new AttackInfo(null,AttackType.Heal,UnityEngine.Random.Range(3,30)));
                if (mcUnit as Steve)
                {
                    (mcUnit as Steve).RandomSpell(false,false);
                }
                Destroy(giftBoxGo,2);
            });
        
        
    }
}
public partial class EMath
{
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        float Func(float x) => 4 * (-height * x * x + height * x);

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        float Func(float x) => 4 * (-height * x * x + height * x);

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t));
    }
}