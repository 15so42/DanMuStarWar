using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicksUi : MaskableGraphic
{
    public int maxHp;
    public int interval;

   
    public float thickness = 1;
    
    public float offsetX = 0;
    public float offsetY = 0;
    
    public void RefreshUi(int maxHp1,int interval1)
    {
        this.maxHp = maxHp1;
        this.interval = interval1;
        if (maxHp > 100)
        {
            interval = 100;
            thickness = 3;
            color=Color.red;
        }
        
        // if (maxHp > 500)
        // {
        //     interval = 50;
        //     color=Color.red;
        // }
        SetVerticesDirty();
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        if(maxHp==0)
            return;
       
        vh.Clear();//清除vh信息

        var count = maxHp / interval;

        Rect rect = GetPixelAdjustedRect();
        for (int i = 0; i < count; i++)
        {
            float x = rect.xMin + (rect.width / count) * i + offsetX;
            float y = rect.yMin + offsetY;

            int indexCount = vh.currentVertCount;
            
            Vector3 p0 = new Vector3(x,y,0);//坐下
            Vector3 p1= new Vector3(x,y+rect.height,0);//左上
            
            Vector3 p2=new Vector3(x+thickness,y+rect.height,0);//右上
            Vector3 p3=new Vector3(x+thickness,y,0);//右下
            
            vh.AddVert(p0,color,Vector2.zero);
            vh.AddVert(p1,color,Vector2.zero);
            vh.AddVert(p2,color,Vector2.zero);
            vh.AddVert(p3,color,Vector2.zero);
            
            vh.AddTriangle(indexCount,indexCount+1,indexCount+2);
            vh.AddTriangle(indexCount,indexCount+2,indexCount+3);
        }
    }

    
}
