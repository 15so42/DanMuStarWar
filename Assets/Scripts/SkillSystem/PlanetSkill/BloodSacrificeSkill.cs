using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/BloodSacrifice")]
public class BloodSacrificeSkill : SkillBase
{
    public int reduceHp=100;
    public int addDicePoint = 7;
    private PlanetResContainer resContainer;
    public override void Init(GameEntity gameEntity,PlanetCommander planetCommander)
    {
        base.Init(gameEntity,planetCommander);
        
        resContainer = gameEntity.GetComponent<PlanetResContainer>();
    }

    protected override void Play()
    {
        base.Play();
        gameEntity.OnAttacked(new AttackInfo(gameEntity,AttackType.Heal, -1*reduceHp));
        resContainer.AddRes(ResourceType.DicePoint,addDicePoint);
        //Debug.Log("治疗");
    }
    
}
