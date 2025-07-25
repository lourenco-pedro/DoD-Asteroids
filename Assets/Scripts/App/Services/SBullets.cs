using System.Collections.Generic;
using App.Data;
using App.Data.Definition;
using App.EventPayloads;
using Core;
using ppl.SimpleEventSystem;
using UnityEngine;

namespace App
{
    public class SBullet : ISimulationAgent, IEventBindable
    {
        public void Setup()
        {
        }

        public void Tick(float deltaTime)
        {
            Bullet[] bullets = DataTable.FromCollection(DataType.Bullets).GetAll<Bullet>();

            if (bullets == null || bullets.Length == 0)
                return;

            foreach (var bullet in bullets)
            {
                if (bullet.IsDisabled)
                    continue;


                MoveBullet(bullet, deltaTime);
                CheckCollisionWithAsteroids(bullet);
            }
        }

        void MoveBullet(Bullet bullet, float deltaTime)
        {
            ScreenBoundaires screenBoundaires = DataTable.FromCollection(DataType.GameSettings).WithId<ScreenBoundaires>(Constants.Screen);

            bullet.position += bullet.forward * bullet.speed * deltaTime;

            if (bullet.position.x < screenBoundaires.min.x || bullet.position.x > screenBoundaires.max.x ||
                bullet.position.y < screenBoundaires.min.y || bullet.position.y > screenBoundaires.max.y)
            {
                bullet.IsDisabled = true;
            }
        }

        void CheckCollisionWithAsteroids(Bullet bullet)
        {
            Asteroid[] asteroids = DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>();
            if (asteroids != null)
            {
                foreach (var asteroid in asteroids)
                {
                    float distance = Vector2.Distance(bullet.position, asteroid.position);
                    float collisionDistance = bullet.radius + asteroid.radius;

                    if (distance <= collisionDistance)
                    {
                        bullet.IsDisabled = true;
                        asteroid.IsDisabled = true;

                        EvtOnAsteroidHitArgs asteroidHitEventArgs = new EvtOnAsteroidHitArgs
                        {
                            fromDataType = DataType.Bullets,
                            asteroidId = asteroid.Id,
                            fromId = bullet.Id,
                            pointsGained = asteroid.points
                        };


                        Wrappers.GameScore.Add(asteroid.points);
                        Wrappers.GameScore.CountAsteroid();

                        //Emmiting hit event so other systems can handle it
                        this.EmitEvent(Constants.EvtAsteroidDestroyed, asteroidHitEventArgs);

                        if (asteroid.size > .5f)
                        {
                            float newSize = asteroid.size * 0.5f;
                            float newRadius = asteroid.radius * 0.5f;

                            if (!DataTable.FromCollection(DataType.Asteroids).Pool(out Asteroid newAsteroid1))
                            {
                                newAsteroid1.Id = string.Format(Constants.AsteroidId, DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>(includeDisabled: true).Length);
                                DataTable.FromCollection(DataType.Asteroids).Register(newAsteroid1);
                            }

                            if (!DataTable.FromCollection(DataType.Asteroids).Pool(out Asteroid newAsteroid2))
                            {
                                newAsteroid2.Id = string.Format(Constants.AsteroidId, DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>(includeDisabled: true).Length);
                                DataTable.FromCollection(DataType.Asteroids).Register(newAsteroid2);
                            }

                            newAsteroid1.position = asteroid.position;
                            newAsteroid1.forward = (Quaternion.Euler(0, 0, Random.Range(-180, 0)) * asteroid.forward).normalized;
                            newAsteroid1.speed = asteroid.speed * .8f;
                            newAsteroid1.size = newSize;
                            newAsteroid1.radius = newRadius;
                            newAsteroid1.points = Mathf.Max(1, Mathf.RoundToInt(asteroid.points * .08f));
                            newAsteroid1.IsDisabled = false;
                            newAsteroid1.isChildAsteroid = true;

                            newAsteroid2.position = asteroid.position;
                            newAsteroid2.forward = (Quaternion.Euler(0, 0, Random.Range(0, 180)) * asteroid.forward).normalized;
                            newAsteroid2.speed = asteroid.speed * .8f;
                            newAsteroid2.size = newSize;
                            newAsteroid2.points = Mathf.Max(1, Mathf.RoundToInt(asteroid.points * .08f));
                            newAsteroid2.radius = newRadius;
                            newAsteroid2.IsDisabled = false;
                            newAsteroid2.isChildAsteroid = true;

                            //Instantiate another meteor to keep space with bigger meteors
                            if (!DataTable.FromCollection(DataType.Asteroids).Pool(out Asteroid newAsteroid))
                            {
                                newAsteroid.Id = string.Format(Constants.AsteroidId, DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>(includeDisabled: true).Length);
                                DataTable.FromCollection(DataType.Asteroids).Register(newAsteroid1);
                            }

                            float size = Random.Range(0.5f, 2.5f);
                            Vector2 forward = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                            Vector2 newAsteroidPosition = GetOffBoundsPosition(asteroid.position, forward);

                            newAsteroid.position = newAsteroidPosition;
                            newAsteroid.forward = forward;
                            newAsteroid.size = size;
                            newAsteroid.speed = Random.Range(1f, 3f);
                            asteroid.points = Mathf.RoundToInt(size * 10);
                        }

                        break;
                    }

                }
            }
        }

        Vector2 GetOffBoundsPosition(Vector2 from, Vector2 direction)
        {
            ScreenBoundaires screenBoundaires = DataTable.FromCollection(DataType.GameSettings).WithId<ScreenBoundaires>(Constants.Screen);
            float tRight = (screenBoundaires.max.x - from.x) / direction.x;
            float tLeft = (screenBoundaires.min.x - from.x) / direction.x;
            float tTop = (screenBoundaires.max.y - from.y) / direction.y;
            float tBottom = (screenBoundaires.min.y - from.y) / direction.y;

            List<float> tValues = new List<float>();

            if (direction.x > 0) tValues.Add(tRight);
            if (direction.x < 0) tValues.Add(tLeft);
            if (direction.y > 0) tValues.Add(tTop);
            if (direction.y < 0) tValues.Add(tBottom);

            float tMin = Mathf.Min(tValues.ToArray());

            return from + direction * tMin;
        }
    }
}