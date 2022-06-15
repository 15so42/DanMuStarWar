using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Posion")]
public class PoisonSkill : SkillBase
{
    public int time=5;

    public GameEntity attacker;
    public int damageValue=1;

    public void SetAttacker(GameEntity gameEntity)
    {
        this.attacker = gameEntity;
    }

    public void SetAttackDamage(int damage)
    {
        this.damageValue = damage;
    }
    
    public override void Init(GameEntity gameEntity,PlanetCommander planetCommander)
    {
        base.Init(gameEntity,planetCommander);
        if(gameEntity==null || (gameEntity as Steve)==null ||  gameEntity.die)
            return;
        (base.gameEntity as Steve).OpenPoisonFx();
        
        
        onFinished+= (base.gameEntity as Steve).ClosePoisonFx;
    }

   

    protected override void Play()
    {
        base.Play();
        gameEntity.OnAttacked(new AttackInfo(attacker,AttackType.Real, damageValue));
        createCommander.attackOtherDamage += damageValue;

    }

}
