using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private UnityTimer.Timer timer;
    // Update is called once per frame
    void Update()
    {
        
    }

    void ReSpawn()
    {
        gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(FightingManager.Instance.gameStatus!=GameStatus.Playing)
            return;
        var gameEntity = other.collider.GetComponent<GameEntity>();
        if (gameEntity==null || gameEntity.die)
        {
            return;
        }

        var steve = gameEntity as Steve;
        if(!steve || steve==null)
            return;

        var random = UnityEngine.Random.Range(0, 3);
        switch (random)
        {
            case 0:
                var addHp = UnityEngine.Random.Range(3,7);
                steve.AddMaxHp(addHp);
                FlyText.Instance.ShowDamageText(steve.transform.position, "生命上限增加:"+addHp);
                break;
            case 1:
                steve.RandomSpell(false,false);
                FlyText.Instance.ShowDamageText(steve.transform.position, "随机附魔");
                break;
            case 2:
                var point = UnityEngine.Random.Range(6, 15);
                (steve.planetCommander as SteveCommander)?.AddPoint(point);
                FlyText.Instance.ShowDamageText(steve.transform.position, "额外点数"+point);
                break;

        }

        timer=UnityTimer.Timer.Register(180, () =>
        {
            ReSpawn();
        });
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        timer?.Cancel();
    }
}
