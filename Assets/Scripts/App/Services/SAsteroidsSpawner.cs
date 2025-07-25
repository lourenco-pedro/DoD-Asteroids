using System.Collections.Generic;
using System.Linq;
using App.Data;
using App.Data.Definition;
using Core;
using ppl.SimpleEventSystem;
using R3;
using UnityEngine;

namespace App
{
    public class SAsteroidsSpawner : ISimulationAgent, IEventBindable
    {
        float currentSpawnTime = 0;

        public void Setup()
        {
        }

        public void Tick(float deltaTime)
        {
            Asteroid[] asteroids = DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>();
            asteroids = asteroids.Where(a => !a.isChildAsteroid).ToArray();

            Scores scores = DataTable.FromCollection(DataType.GameSettings).WithId<Scores>(Constants.GameScore);
            GameConfiguration gameConfiguration = DataTable.FromCollection(DataType.GameSettings).WithId<GameConfiguration>(Constants.GameConfiguration);

            int currentAsteroidsDifficultyTier = EvaluateTier(scores.asteroidsDestroyed);
            float spawnDuration = gameConfiguration.asteroidSpawnRateInSeconds * gameConfiguration.asteroidsSpawnRateMultiplerTier[currentAsteroidsDifficultyTier];
            int maxActiveAsteroidsTierIndex = Mathf.Min(currentAsteroidsDifficultyTier, Mathf.Max(0, gameConfiguration.maxActiveAsteroidsTier.Length - 1));

            if (asteroids.Length < gameConfiguration.maxActiveAsteroidsTier[maxActiveAsteroidsTierIndex])
            {
                currentSpawnTime += deltaTime;
                currentSpawnTime = Mathf.Lerp(currentSpawnTime, spawnDuration, deltaTime / spawnDuration);
                if (currentSpawnTime >= spawnDuration)
                {
                    currentSpawnTime = 0;
                    SpawnAAsteroid();
                }
            }
        }

        int EvaluateTier(int asteroidsDestroyed)
        {
            GameConfiguration gameConfiguration = DataTable.FromCollection(DataType.GameSettings).WithId<GameConfiguration>(Constants.GameConfiguration);
            for (int i = 0; i < gameConfiguration.asteroidsCountProgression.Length; i++)
            {
                if (asteroidsDestroyed < gameConfiguration.asteroidsCountProgression[i])
                {
                    return i;
                }
            }

            return gameConfiguration.asteroidsCountProgression.Length - 1;
        }

        Asteroid SpawnAAsteroid()
        {
            ScreenBoundaires screenBoundaires = DataTable.FromCollection(DataType.GameSettings).WithId<ScreenBoundaires>(Constants.Screen);
            GameConfiguration gameConfiguration = DataTable.FromCollection(DataType.GameSettings).WithId<GameConfiguration>(Constants.GameConfiguration);
            Asteroid[] asteroids = DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>();

            float size = Random.Range(gameConfiguration.asteroidSizeRange[0], gameConfiguration.asteroidSizeRange[1]);

            float x = Random.Range(screenBoundaires.min.x, screenBoundaires.max.x);
            float y = Random.Range(screenBoundaires.min.y, screenBoundaires.max.y);
            Vector2 forward = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Vector2 initialPosition = new Vector2(x, y);

            if (!DataTable.FromCollection(DataType.Asteroids).Pool(out Asteroid asteroid))
            {
                asteroid.Id = string.Format(Constants.AsteroidId, asteroids.Length);
                DataTable.FromCollection(DataType.Asteroids).Register(asteroid);
            }

            asteroid.isChildAsteroid = false;
            asteroid.points = Mathf.RoundToInt(size * 10);
            asteroid.size = size;
            asteroid.position = GetOffBoundsPosition(initialPosition, -forward);
            asteroid.speed = Random.Range(gameConfiguration.asteroidsSpeedRange[0], gameConfiguration.asteroidsSpeedRange[1]);
            asteroid.forward = forward;
            return asteroid;
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