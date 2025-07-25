using System.Linq;
using App.Data;
using App.Data.Definition;
using Core;
using ppl.SimpleEventSystem;
using R3;
using UnityEngine;

namespace App
{
    public class SAsteroids : ISimulationAgent, IEventBindable
    {
        public void Setup()
        {
        }

        public void Tick(float deltaTime)
        {
            Asteroid[] asteroids = DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>();

            float offset = 0.5f;

            foreach (var asteroid in asteroids)
            {
                asteroid.position += asteroid.forward * asteroid.speed * deltaTime;

                ScreenBoundaires screenBoundaires = DataTable.FromCollection(DataType.GameSettings).WithId<ScreenBoundaires>(Constants.Screen);

                float minX = screenBoundaires.min.x - offset;
                float maxX = screenBoundaires.max.x + offset;
                float minY = screenBoundaires.min.y - offset;
                float maxY = screenBoundaires.max.y + offset;

                if (asteroid.position.x < minX || asteroid.position.x > maxX ||
                    asteroid.position.y < minY || asteroid.position.y > maxY)
                {
                    asteroid.position = new Vector2(
                        Mathf.Repeat(asteroid.position.x - minX, maxX - minX) + minX,
                        Mathf.Repeat(asteroid.position.y - minY, maxY - minY) + minY
                    );
                }

                CheckCollision(asteroid);
            }
        }

        void CheckCollision(Asteroid asteroid)
        {
            Player[] players = DataTable.FromCollection(DataType.Players).GetAll<Player>();

            foreach (Player player in players)
            {
                float distance = Vector2.Distance(player.position, asteroid.position);
                float collisionDistance = player.radius + asteroid.radius;

                if (distance <= collisionDistance)
                {
                    asteroid.IsDisabled = true;
                    player.life--;
                    int totalLife = players.Select(p => p.life).Sum();

                    if (player.life <= 0)
                        DataTable.FromCollection(DataType.Players).RemoveId(player.Id);

                    this.EmitEvent(Constants.EvtOnLifeChange, totalLife);
                }
            }
        }
    }
}