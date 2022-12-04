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
    public List<string> list;
    public int count;
    public int interval;
    public SpawnMonsterBehaviour template = new SpawnMonsterBehaviour();
 
    
 
    //ClipCaps是必须实现的一个属性，代表了你的Clip支持哪些功能，并影响你编辑器对Clip的操作。
    //比如，Blending代表你的Clip支持融合。在编辑器中，你可以将两个Clip拖到共同时间段，并且你可以编辑融合的融合曲线。
    //如果开启了融合，每个时间段Clip（这里是指CustomPlayableBehaviour）的权重不只是0和1.可能同一时间两个Clip的权重都是0.x。（当然，你要自己根据权重实现相应的融合逻辑，不然一切都没意义。）
    //在IDE中，看ClipCaps的声明可以了解更多
    public ClipCaps clipCaps { get; }
 
    //重写这个工厂方法，播放轨道的时候就会创建ScriptPlayable<T>，由一小节关于playable，ScriptPlayable是由
    //playablebehaviour驱动的一种特殊的playable（同时也是个接口体）
    public override Playable CreatePlayable (PlayableGraph graph,GameObject owner)
    {
        var playable = ScriptPlayable<SpawnMonsterBehaviour>.Create(graph,template);
 
        SpawnMonsterBehaviour behaviour = playable.GetBehaviour();
        behaviour.count = count;
        behaviour.list = list;
        behaviour.interval = interval;
        clipBoard = "BattleUnit_Zombie\nBattleUnit_Skeleton\nBattleUnit_Creeper\nBattleUnit_BattleUnit_Blaze\n" +
                    "BattleUnit_EvilIronGolem\nBattleUnit_Wither\nBattleUnit_EnderDragon";
        return playable;
    }

    
}
