using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public string skillName;

    public int cd;//冷却时间

    [Header("可使用次数,-1表示无限次")]
    public int life=-1;

    public abstract void Play(GameEntity entity);

}
