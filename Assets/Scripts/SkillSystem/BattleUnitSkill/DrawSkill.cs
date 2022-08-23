using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//汲取
[CreateAssetMenu(fileName = "Skill/Draw")]
public class DrawSkill : SkillBase
{

    public GameEntity attacker;
    public int damage = 1;

    public void SetAttacker(GameEntity gameEntity)
    {
        this.attacker = gameEntity;
    }
    
    public override void Init(GameEntity gameEntity,PlanetCommander planetCommander)
    {
        base.Init(gameEntity,planetCommander);
        if(gameEntity==null || (gameEntity as McUnit)==null ||  gameEntity.die)
            return;
        (base.gameEntity as McUnit).OpenDrawFx();
        
        
        onFinished+= (base.gameEntity as McUnit).CloseDrawFx;
    }

   

    protected override void Play()
    {
        base.Play();
        
        gameEntity.OnAttacked(new AttackInfo(attacker,AttackType.Real, damage));
        if (createCommander != null)
        {
            createCommander.attackOtherDamage += damage;
        }
        
        //(gameEntity as Steve)?.GetActiveWeapon()?.AddEndurance(-1);
        attacker.OnAttacked(new AttackInfo(attacker, AttackType.Heal, damage));
    }

}
