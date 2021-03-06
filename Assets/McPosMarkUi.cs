using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class McPosMarkUi : MonoBehaviour
{
    public Text text;

    public  Vector3 offset=new  Vector3(0,25,0);
    public GameObject obj;

    private Camera mainCamera;

    private int index;

    public Text tipText;

    private string tipStr;
    // Start is called before the first frame update

    private FightingManager fightingManager;

    public void Init(GameObject obj, int index,string tip,Vector3 offset)
    {
        this.obj = obj;
        this.index = index;
        EventCenter.AddListener(EnumEventType.OnPlanetsSpawned,Show);
        EventCenter.AddListener(EnumEventType.OnStartWaitingJoin,Hide);
        gameObject.SetActive(false);
        tipText.gameObject.SetActive(true);
        this.tipStr = tip;
        this.offset = offset;

    }
    void Start()
    {
        mainCamera=Camera.main;
        this.text.text = "> " + index + " <";
        tipText.text = tipStr;
        fightingManager = GameManager.Instance.fightingManager;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = mainCamera.WorldToScreenPoint(obj.transform.position) + offset;
        
        
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        
    }
}
