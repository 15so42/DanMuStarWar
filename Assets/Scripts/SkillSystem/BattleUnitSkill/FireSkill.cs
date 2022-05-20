using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Fire")]
public class FireSkill : SkillBase
{
    public int time=5;
    
    public override void Init(GameEntity gameEntity,PlanetCommander planetCommander)
    {
        base.Init(gameEntity,planetCommander);
        if(gameEntity==null || (gameEntity as Steve)==null ||  gameEntity.die)
            return;
        (base.gameEntity as Steve).OpenFireFx();
        
        
        onFinished+= (base.gameEntity as Steve).CloseFireFx;
    }

   

    protected override void Play()
    {
        base.Play();
        gameEntity.OnAttacked(new AttackInfo(null,AttackType.Physics, 1));
        createCommander.attackOtherDamage += 1;

    }

}
