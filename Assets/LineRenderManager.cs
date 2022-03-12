using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderManager : MonoBehaviour
{
    public GameObject lineRenderPfb;

    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent=new GameObject("==========LineRenders========");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLineRender(Vector3 startPos, Vector3 endPos)
    {
        GameObject lineRender=GameObject.Instantiate(lineRenderPfb,parent.transform);
        lineRender.GetComponent<LineRenderer>().SetPositions(new[]{startPos,endPos});
    }
}
