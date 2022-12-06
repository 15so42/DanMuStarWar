using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityTimer;

// A behaviour that is attached to a playable
public class SpawnMonsterBehaviour : PlayableBehaviour
{

    public int interval = 30;
    public List<string> list=new List<string>();
    public int count;
    public bool loop;

    private UnityTimer.Timer spawnTimer;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
       
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        spawnTimer?.Cancel();
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.LogError("OnBehaviorPlay");
        
        spawnTimer = UnityTimer.Timer.Register(interval, () =>
        {
            PVEManager.Instance.SetMonsterList(list);
            PVEManager.Instance.SpawnByPlayerCount(count);
        }, null, loop);
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
        spawnTimer?.Cancel();
    }
    
    

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
