using System.Collections.Generic;
using System.Linq;
using App.Data;
using App.Data.Definition;
using App.Render.WorldObjects;
using UnityEngine;

namespace App.Render
{
    public class RBullets : Core.IRenderAgent
    {
        Dictionary<string, WOBullet> _bullets;

        public void Setup()
        {
            if(null == _bullets)
                _bullets = new Dictionary<string, WOBullet>();
        }

        public void Draw()
        {
            var activeBullets = DataTable.FromCollection(DataType.Bullets).GetAll<Bullet>();
            var inactiveBullets = DataTable.FromCollection(DataType.Bullets).GetAll<Bullet>(includeDisabled: true).Except(activeBullets);

            string[] keysToRemove = _bullets.Keys.Except(
                activeBullets.Select(a => a.Id).Concat(
                    inactiveBullets.Select(i => i.Id)
                )
            ).ToArray();

            foreach (string id in keysToRemove)
            {
                GameObject.Destroy(_bullets[id].GameObject);
                _bullets.Remove(id);
            }

            foreach (var bullet in activeBullets)
            {
                if (!_bullets.TryGetValue(bullet.Id, out WOBullet worldObject))
                {
                    worldObject = new WOBullet(bullet.Id);
                    _bullets.Add(bullet.Id, worldObject);
                }

                worldObject.Position = bullet.position;
                worldObject.CheckDisabled();
            }

            foreach (var bullet in inactiveBullets)
            {
                if (_bullets.TryGetValue(bullet.Id, out WOBullet worldObject))
                {
                    worldObject.CheckDisabled();
                }
            }
        }
    }
}