using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/凋零")]
public class WitherSkill : SkillBase
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
        var mcUnit=base.gameEntity as McUnit;
        if(mcUnit==null)
            return;
        var previousColor = mcUnit.hpUI.hpFill.color;
        mcUnit.hpUI.hpFill.color=Color.black;
        
        
        onFinished+= ()=>
        {
            mcUnit.hpUI.hpFill.color = previousColor;

        };
    }

   

    protected override void Play()
    {
        base.Play();
        gameEntity.OnAttacked(new AttackInfo(attacker,AttackType.Real, damage));
        if (createCommander != null)
        {
            if((gameEntity as BattleUnit)?.planetCommander!=createCommander)//对自己造成伤害不算进去
                createCommander.attackOtherDamage += damage;
           
        }

    }

}
