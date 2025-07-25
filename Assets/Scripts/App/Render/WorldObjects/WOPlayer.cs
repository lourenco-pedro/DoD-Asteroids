using App.AssetBooks;
using App.Data;
using App.Data.Definition;
using Core;
using UnityEngine;

namespace App.Render.WorldObjects
{
    public class WOPlayer : BaseWorldObject
    {
        public WOPlayer(string id) : base(id)
        {
            Player p = DataTable.FromCollection(DataType.Players).WithId<Player>(id);

            GameObject.transform.position = p.position;
            GameObject.transform.rotation = Quaternion.Euler(0, 0, p.rotation);

            Sprite shipSprite = AssetBook.Get<SpriteAssetBook>(Constants.MainSpritesAssetBook).GetAsset(SpriteAssetBook.PlayerShip);
            Render.sprite = shipSprite;

            p.radius = Render.bounds.extents.x;
        }

        public void SetColor(Color color)
        {
            Render.color = color;
        }
    }
}