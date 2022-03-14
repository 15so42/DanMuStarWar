using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWeapon : Weapon
{
    public GameObject lineRendererPfb;
    
    
   

    public override void FireFx(Vector3 startPos, Vector3 endPos)
    {
        LineRenderManager.Instance.SetLineRender(transform.position, endPos, lineRendererPfb);
    }
}
