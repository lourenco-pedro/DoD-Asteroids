using System.Collections.Generic;
using System.Linq;
using App.Data;
using App.Data.Definition;
using App.Render.WorldObjects;
using Core;
using UnityEngine;

namespace App.Render
{
    public class RAsteroids : IRenderAgent
    {
        Dictionary<string, WOAsteroid> _asteroids;

        public void Setup()
        {
            if(null == _asteroids)
                _asteroids = new Dictionary<string, WOAsteroid>();
        }

        public void Draw()
        {
            Asteroid[] activeAsteroids = DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>(includeDisabled: false);
            Asteroid[] inactiveAsteroids = DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>(includeDisabled: true).Except(activeAsteroids).ToArray();

            string[] keysToRemove = _asteroids.Keys.Except(
                activeAsteroids.Select(a => a.Id).Concat(
                    inactiveAsteroids.Select(i => i.Id)
                )
            ).ToArray();

            foreach (string id in keysToRemove)
            {
                GameObject.Destroy(_asteroids[id].GameObject);
                _asteroids.Remove(id);
            }

            foreach (Asteroid asteroid in activeAsteroids)
            {
                if (!_asteroids.TryGetValue(asteroid.Id, out WOAsteroid worldObject))
                {
                    worldObject = new WOAsteroid(asteroid.Id);
                    _asteroids.Add(asteroid.Id, worldObject);
                }

                asteroid.radius = worldObject.Render.bounds.extents.x;
                worldObject.Position = asteroid.position;
                worldObject.GameObject.transform.localScale = new Vector3(asteroid.size, asteroid.size, 1);

                worldObject.CheckDisabled();
            }

            foreach (Asteroid asteroid in inactiveAsteroids)
            {
                if (_asteroids.TryGetValue(asteroid.Id, out WOAsteroid worldObject))
                    worldObject.GameObject.SetActive(!asteroid.IsDisabled);
            }
        }
    }
}