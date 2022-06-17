using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Fire")]
public class FireSkill : SkillBase
{
    public int time=5;

    public GameEntity attacker;

    public void SetAttacker(GameEntity gameEntity)
    {
        this.attacker = gameEntity;
    }
    
    public override void Init(GameEntity gameEntity,PlanetCommander planetCommander)
    {
        base.Init(gameEntity,planetCommander);
        if(gameEntity==null || (gameEntity as McUnit)==null ||  gameEntity.die)
            return;
        (base.gameEntity as McUnit).OpenFireFx();
        
        
        onFinished+= (base.gameEntity as McUnit).CloseFireFx;
    }

   

    protected override void Play()
    {
        base.Play();
        gameEntity.OnAttacked(new AttackInfo(attacker,AttackType.Physics, 1));
        if (createCommander != null)
        {
            createCommander.attackOtherDamage += 1;
        }
        
        (gameEntity as Steve)?.GetActiveWeapon()?.AddEndurance(-1);

    }

}
