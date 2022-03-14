using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCode.Tools;
using UnityEngine;

public class BulletWeapon : Weapon
{
   public override void FireFx(Vector3 startPos, RaycastHit hitInfo)
   {
      base.FireFx(startPos, hitInfo);
      if(hitInfo.collider.transform==owner.transform)
         return;
      GameObject fx = ResFactory.Instance.CreateFx(GameConst.FX_BULLET_HIT, hitInfo.point);
   }

   public override void FireFx(Vector3 startPos, Vector3 endPos)
   {
      base.FireFx(startPos, endPos);
      GameObject bullet = ResFactory.Instance.CreateBullet(GameConst.BULLET_NORMAL,startPos);
      bullet.GetComponent<Bullet>().SetEndPos(endPos);
      Vector3 dir = endPos - startPos;
      bullet.transform.forward = dir;
      

   }
}
