using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnowWeather : MonoBehaviour
{
    public Vector2 areaStart;

    public Vector2 areaEnd;

    private float timer = 0;

    public List<string> snowBlockList;

    public float scale = 3;
    [Header("原点")]
    public Vector3 offset;
    private int[,] grids;
    
    // Start is called before the first frame update
    private int xCount = 0;
    private int yCount = 0;

    private List<GameObject> snowCubes=new List<GameObject>();
    
    void Start()
    {
        xCount = (int)((areaEnd.x - areaStart.x) / scale);
        yCount = (int)((areaEnd.y - areaStart.y) / scale);
        grids=new int[xCount,yCount];
        offset+=new Vector3(areaStart.x,100,areaStart.y);

        SceneManager.sceneUnloaded += ClearAllSnowCube;
    }

   

    Vector2Int GetRandLogicPos()
    {
        var randX = UnityEngine.Random.Range(0, xCount);
        var randY = UnityEngine.Random.Range(0, yCount);
       
        return new Vector2Int(randX,randY);
        
      
    }

    Vector3 GetWorldPosByIndex(Vector2Int logicPos)
    {
        return offset + new Vector3(logicPos.x * scale, 0, logicPos.y * scale);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            var randLogicPos = GetRandLogicPos();
            if ( grids[randLogicPos.x,randLogicPos.y]>0)
            {
                return;//已经有雪了
            }
            
            

            var worldPos = GetWorldPosByIndex(randLogicPos);
            RaycastHit hitInfo;
            //向下投影
            if (Physics.Raycast(worldPos, Vector3.down, out hitInfo, 300))
            {
                if (snowBlockList.Contains(hitInfo.collider.name))
                {
                    GameObject go=ResFactory.Instance.CreateFx("SnowCubeFx", hitInfo.point);
                    go.transform.SetParent(gameObject.transform);
                    snowCubes.Add(go);
                    grids[randLogicPos.x, randLogicPos.y] = 1;
                }
            }
            

            timer = 0;
        }
    }

    void ClearAllSnowCube(Scene arg0)
    {
        for (int i = 0; i < snowCubes.Count; i++)
        {
            //因为是异步的，所以不用i--
            Destroy(snowCubes[i].gameObject);
        }

        SceneManager.sceneUnloaded -= ClearAllSnowCube;

    }

    
}
