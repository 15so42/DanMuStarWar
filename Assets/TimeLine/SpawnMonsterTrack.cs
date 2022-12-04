using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


[Serializable]//保证序列化
[TrackClipType(typeof(SpawnMonsterClip))]//表示Track添加哪种Clip
[TrackBindingType(typeof(PVEManager))]//表示Track绑定哪种类型的对象（GameObject或者任何Component等等）
[TrackColor(0.53f,0.0f,0.08f)]//表示在编辑器中，Track轨道前端的标识颜色（不重要啦）
public class SpawnMonsterTrack : TrackAsset
{
    
}
