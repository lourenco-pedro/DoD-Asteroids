using App.AssetBooks;
using App.Data;
using App.Data.Definition;
using Core;
using UnityEngine;

namespace App.Render.WorldObjects
{
    public class WOAsteroid : BaseWorldObject
    {
        public WOAsteroid(string id) : base(id)
        {
            Asteroid asteroid = DataTable.FromCollection(DataType.Asteroids).WithId<Asteroid>(id);

            GameObject.transform.position = asteroid.position;
            GameObject.transform.localScale = new Vector3(asteroid.size, asteroid.size, 1);

            Sprite asteroidSprite = AssetBook.Get<SpriteAssetBook>(Constants.MainSpritesAssetBook).GetAsset(SpriteAssetBook.Asteroid);
            Render.sprite = asteroidSprite;

            asteroid.radius = asteroidSprite.bounds.extents.x;
        }
        
        public override void CheckDisabled()
        {
            Asteroid asteroid = DataTable.FromCollection(DataType.Asteroids).WithId<Asteroid>(Id);
            GameObject.SetActive(asteroid != null && !asteroid.IsDisabled);
        }
    }
}