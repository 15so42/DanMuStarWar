using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatisItemUi : MonoBehaviour
{
    private PlanetCommander planetCommander;
    public Image face;
    public Text nameText;
    public Text kd;
    public Text damage;
    public Text victimDamage;
    public Text heal;

    public void Init(PlanetCommander planetCommander)
    {
        // try
        // {
            this.planetCommander = planetCommander;
            var player = planetCommander.player;
            face.sprite = player.faceSprite;
            nameText.text = player.userName;
            kd.text = this.planetCommander.slainCount + "/" + planetCommander.dieCount;
            damage.text = planetCommander.attackOtherDamage + "";
            victimDamage.text = planetCommander.attackedDamage + "";
            heal.text = this.planetCommander.healSelfValue + "";
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError(e);
        // }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
