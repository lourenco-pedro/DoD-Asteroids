using UnityEngine;
using Core;
using App.Data;
using App.Data.Definition;

namespace App.Render.WorldObjects
{

    public class WOBullet : BaseWorldObject
    {
        public WOBullet(string id) : base(id)
        {
            Bullet bullet = DataTable.FromCollection(DataType.Bullets).WithId<Bullet>(id);

            GameObject.transform.position = bullet.position;

            Sprite bulletSprite = AssetBook.Get<AssetBooks.SpriteAssetBook>(Constants.MainSpritesAssetBook).GetAsset(AssetBooks.SpriteAssetBook.Bullet);
            Render.sprite = bulletSprite;

            bullet.radius = bulletSprite.bounds.extents.x;            
        }

        public override void CheckDisabled()
        {
            Bullet bullet = DataTable.FromCollection(DataType.Bullets).WithId<Bullet>(Id);
            GameObject.SetActive(bullet != null && !bullet.IsDisabled);
        }
    }
}