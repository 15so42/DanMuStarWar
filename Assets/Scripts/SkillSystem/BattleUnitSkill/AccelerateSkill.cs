using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Accelerate")]
public class AccelerateSkill : BuffBase
{
    public float addMoveSpeed=4;
    public float duration = 3;
    private MoveManager moveManager;

    public void SetParams(float addSpeed,float duration)
    {
        
    }
    public override void Init(GameEntity gameEntity,PlanetCommander planetCommander)
    {
        base.Init(gameEntity,planetCommander);
        moveManager = gameEntity.GetComponent<MoveManager>();
        if (moveManager)
        {
            //moveManager.curSpeed += addMoveSpeed;
            moveManager.StartBoost(addMoveSpeed);
            UnityTimer.Timer.Register(duration, () =>
            {
                //moveManager.curSpeed -= addMoveSpeed;
                moveManager.EndBoost(addMoveSpeed);
                ChangeLife(--life);
                
            });
        }
    }

    
    
    
}
