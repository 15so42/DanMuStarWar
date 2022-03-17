using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Heal")]
public class HealSkill : SkillBase
{
    public int healValue=1;
    private BattleUnitProps prop;
    public override void Init(GameEntity gameEntity)
    {
        base.Init(gameEntity);
        prop=gameEntity.GetComponent<BattleUnitProps>();
    }

    protected override void Play()
    {
        base.Play();
        gameEntity.OnAttacked(new AttackInfo(gameEntity,AttackType.Heal, healValue));
        //Debug.Log("治疗");
    }
    
}
