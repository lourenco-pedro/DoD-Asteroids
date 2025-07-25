using UnityEngine;

namespace App.AssetBooks
{
    [CreateAssetMenu(fileName = "AssetBook", menuName = "AssetBook/Sprite", order = 1)]
    public class SpriteAssetBook : AssetBook<Sprite>
    {
        public const string PlayerShip = "player";
        public const string Asteroid = "asteroid";
        public const string Bullet = "bullet";
    }
}