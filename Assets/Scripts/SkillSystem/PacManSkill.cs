using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Planet/PacMan")]
public class PacManSkill : SkillBase
{
   protected override void Play()
   {
      base.Play();
      (gameEntity as Planet).AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_吃豆人",1)));
   }
}
