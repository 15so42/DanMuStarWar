using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Fire")]
public class FireSkill : SkillBase
{
    public int time=5;

    public GameEntity attacker;
    //public int damage = 1;
    

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
        var fireLevel = life - 3>0?life-3:1;
        var damage = Mathf.CeilToInt(gameEntity.props.hp * (0.01f));
        gameEntity.OnAttacked(new AttackInfo(attacker,AttackType.Real, damage));
        if (createCommander != null)
        {
            if((gameEntity as BattleUnit)?.planetCommander!=createCommander)//僵尸燃烧不能算到上海里去
                createCommander.attackOtherDamage += damage;
           
        }

        var steve = gameEntity as Steve;
        if (steve!=null)
        {
            var weapon = steve.GetActiveWeapon();
            if (weapon)
            {
                weapon.AddEndurance(-1*Mathf.CeilToInt(weapon.endurance*0.01f));
            }
        }
        

    }

}
