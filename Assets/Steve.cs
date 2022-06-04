using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.Rendering;

public class Steve : McUnit
{

    private int curWeaponId;//切换武器时更新

    [HideInInspector] public string cs64Code = "";
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    [Header("换肤材质球")] public List<SkinnedMeshRenderer> skinMeshRenderers;
    [Header("本地皮肤列表")] public List<Texture> skinTextures;
    
    [Header("普通武器")]
    public List<HandWeapon> normalWeapons=new List<HandWeapon>();
    public List<HandWeapon> rareWeapons=new List<HandWeapon>();

    protected override void Start()
    {
        base.Start();
       
        //RandomWeapon();
        hpUI.SetNameText(planetCommander.player.userName);
        hpUI.OpenHPTile();
        

        if (planetCommander is SteveCommander commander)
        {
            commander.battleUnits.Add(this);
            if (commander.desireMaxHp != 0)
            {
                AddMaxHp(commander.desireMaxHp - props.maxHp);
            }
        }

        var liveWeapon = InitDesireWeapon();//读档，如果读取是空手就随机武器
        // if (liveWeapon==null ||  (liveWeapon as HandWeapon).mcWeaponId == 0)//空手
        // {
        //     RandomWeapon();
        // }
        
        //todo 删除测试
        //ChangeWeapon(7);
        
        //关闭碰撞体
        //trigger.enabled = false;


        planetCommander.player.onSetUserData += OnPlayerSetData;
        
        //如果在Steve生成前已经就触发了onSetUserData,生成时就不会触发，所以需要手动触发
        OnPlayerSetData();
    }

    public void OnPlayerSetData()
    {
        //使用皮肤,异步
        var steveCommander = planetCommander as SteveCommander;
        if(steveCommander.player.userSaveData==null)
            return;
        if (steveCommander.player.userSaveData.skinId == -1)
        {
            SetSkin(steveCommander.player.userSaveData.customSkin64Code);
        }
        else
        {
            SetSkinById(steveCommander.player.userSaveData.skinId);
        }
    }

    void SetSkin(string cs64Code)
    {
        Texture2D _texture = new Texture2D(64, 32);
        _texture.LoadImage(Convert.FromBase64String(cs64Code));
        _texture.filterMode = FilterMode.Point;
        for (int i = 0; i < skinMeshRenderers.Count; i++)
        {
            skinMeshRenderers[i].material.SetTexture(MainTex,_texture);
        }
        
    }

    void SetSkinById(int index)
    {
        for (int i = 0; i < skinMeshRenderers.Count; i++)
        {
            skinMeshRenderers[i].material.SetTexture(MainTex,skinTextures[index]);
        }
    }

    

    public Weapon InitDesireWeapon()
    {
        var steveCommander = (planetCommander as SteveCommander);
        if (steveCommander == null)
            return null;
        
        var liveWeapon=ChangeWeapon(steveCommander.desireWeaponId);
        (liveWeapon as HandWeapon).Load(steveCommander.steveWeaponNbt);

        return liveWeapon; 
    }

    public void UpdateWeaponEndurance(int endurance,int maxEndurance)
    {
        if (hpUI && hpUI.gameObject)
        {
            hpUI.UpdateWeaponEndurance(endurance, maxEndurance);
        }
        
        if (endurance <= 0)
        {
            //RandomWeapon();
        }
    }
    
    public void FixWeapon(int value)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        (liveWeapon as HandWeapon).AddEndurance(value);
    }

    public Weapon ChangeWeapon(int weaponId)
    {
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        var liveWeapon = weapons.Find(x => (x as HandWeapon).mcWeaponId == weaponId);
        
        liveWeapon.gameObject.SetActive(true);
        liveWeapon.Init(this);
        hpUI.SetWeaponText(liveWeapon.weaponName);
        curWeaponId = weaponId;
        
        
        
        //(liveWeapon as HandWeapon)?.SaveToCommander();
        return liveWeapon;
    }

    public void RandomRareWeapon()
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        while (true)
        {
            var random = rareWeapons[UnityEngine.Random.Range(0, rareWeapons.Count)].mcWeaponId;
            if (random != (liveWeapon as HandWeapon).mcWeaponId)
            {
                ChangeWeapon(random);
                break;
            }
        }

    }

    public void RandomWeapon()
    {
        if(die)
            return;
        
        Debug.Log("抽取武器");
        var steveCommander = (planetCommander as SteveCommander);
        if (steveCommander != null && steveCommander.point > 6)
        {
            
            var randomId=normalWeapons[ UnityEngine.Random.Range(1, normalWeapons.Count)].mcWeaponId;
            var liveWeapon = ChangeWeapon(randomId);
            MessageBox._instance.AddMessage("[系统]" + planetCommander.player.userName + "抽取武器:" + liveWeapon.weaponName);
            
            hpUI.SetWeaponText(liveWeapon.weaponName);
            steveCommander.AddPoint(-6);

        }
        else
        {
            MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"自动抽取失败，点数不足");
            // var liveWeapon = ChangeWeapon(0);
            // hpUI.SetWeaponText(liveWeapon.weaponName);
        }
    }

    public bool TryRandomSpell(bool byGift)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        return (liveWeapon as HandWeapon).TryRandomSpell(byGift);
    }
    
    public bool TrySpecificSpell(string spellName)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        return (liveWeapon as HandWeapon).TrySpecificSpell(spellName);
    }
    
    
    
    public void RandomSpell(bool rare,bool gift)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        var weaponNbt = (liveWeapon as HandWeapon).weaponNbt;
        if(weaponNbt==null)
            return;
        
        if (weaponNbt.enhancementLevels.Count >= weaponNbt.maxSpellCount && !gift)
        {
            (liveWeapon as HandWeapon).OnlyUpdateSpell();
        }
        else
        {
            (liveWeapon as HandWeapon).RandomSpell(rare);
        }
        
    }
    
    public bool SpecificSpell(bool rare,string name)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        if (liveWeapon == null)
            return false;
        (liveWeapon as HandWeapon)?.SpecificSpell(name);
        return true;
    }


    public bool RemoveSpell(int index)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);

        if (liveWeapon == null || liveWeapon as HandWeapon == null)
            return false;
        return (liveWeapon as HandWeapon).RemoveSpell(index);
        
    }
    
    

    

    public void OnBuyWeaponSuccess(string weaponName)
    {
        var targetWeapon = weapons.Find(x => x.weaponName == weaponName);
        var id = (targetWeapon as HandWeapon).mcWeaponId;
        ChangeWeapon(id);
    }
    
    
   

    public override void AddMaxHp(int value)
    {
        base.AddMaxHp(value);
        (planetCommander as SteveCommander).desireMaxHp = props.maxHp;
    }

   

   
    
    

    //进入防御点位后，武器的攻击距离增加
    public void EnterDefendState(int findDistance,int attackDistance)
    {
        trigger.radius += findDistance;
        findEnemyDistance += findDistance;
        canPushBack = false;
        
        float minDistance = 10000;
        foreach (var w in weapons)
        {
            if(w.gameObject.activeSelf==false)
                continue;
            if (w.addAtkDistanceByDP)
            {
                w.attackDistance += attackDistance;
            }
            if (w.attackDistance < minDistance)
                minDistance = w.attackDistance;
            
        }
        
        SetAttackDistance(minDistance);
    }

    public void ExitDefendState(int findDistance,int attackDistance)
    {
        trigger.radius -= findDistance;
        findEnemyDistance -= findDistance;
        canPushBack = true;
        
        float minDistance = 10000;
        foreach (var w in weapons)
        {
            if(w.gameObject.activeSelf==false)
                continue;
            if (w.addAtkDistanceByDP)
            {
                w.attackDistance -= attackDistance;
            }
            if (w.attackDistance < minDistance)
                minDistance = w.attackDistance;
                
            SetAttackDistance(minDistance);
        }
    }


    public override void Die()
    {
        planetCommander.AddPoint(1);
        (planetCommander as SteveCommander).battleUnits.Remove(this);
        EventCenter.Broadcast(EnumEventType.OnSteveDied,this);
        
        //保存武器
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        (liveWeapon as HandWeapon).SaveToCommander();
        
        fightingManager.AddPlayerDataValue(planetCommander.player.uid,"dieCount",1);
        
        planetCommander.player.onSetUserData = null;
        base.Die();
        
    }
}
