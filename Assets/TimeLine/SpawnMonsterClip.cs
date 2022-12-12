using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class SpawnMonsterClip : PlayableAsset,ITimelineClipAsset
{
    //从命名可以看出，这里template一个模板，Clip会在运行时，根据你赋值好的template再创建出一个新的对象。
    //下面会说到，CustomPlayableBehaviour我们最好只放数据，逻辑由mixer来实现
    
    [Multiline(12)]
    [Header("用于复制粘贴怪物名称")]
    public string clipBoard;
    //public List<string> list;
    [TextArea]
    public string spawnList;
    public bool overrideCount;
    public int count;
    public int interval;
    public bool loop = true;
    public SpawnMonsterBehaviour template = new SpawnMonsterBehaviour();

    public SpawnMonsterClip(ClipCaps clipCaps)
    {
        this.clipCaps = clipCaps;
    }


    //重写这个工厂方法，播放轨道的时候就会创建ScriptPlayable<T>，由一小节关于playable，ScriptPlayable是由
    //playablebehaviour驱动的一种特殊的playable（同时也是个接口体）
    public override Playable CreatePlayable (PlayableGraph graph,GameObject owner)
    {
        var playable = ScriptPlayable<SpawnMonsterBehaviour>.Create(graph,template);
 
        SpawnMonsterBehaviour behaviour = playable.GetBehaviour();
        behaviour.overrideCount = overrideCount;
        behaviour.count = count;
        //behaviour.list = list;
        behaviour.spawnList = spawnList;
        behaviour.interval = interval;
        behaviour.loop = loop;
        clipBoard = "BattleUnit_Zombie\nBattleUnit_Skeleton\nBattleUnit_Creeper\nBattleUnit_BattleUnit_Blaze\n" +
                    "BattleUnit_EvilIronGolem\nBattleUnit_Wither\nBattleUnit_EnderDragon";
        return playable;
    }


    public ClipCaps clipCaps { get; }
}
