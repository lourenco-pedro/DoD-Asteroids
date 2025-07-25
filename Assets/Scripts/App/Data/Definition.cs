using UnityEngine;

namespace App.Data.Definition
{
    public enum DataType
    {
        Players,
        Asteroids,
        Bullets,
        GameSettings,
    }
    public interface IData
    {
        public string Id { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class Scores : IData
    {
        public string Id { get; set; }
        public bool IsDisabled { get; set; }
        public int totalScore;
        public int asteroidsDestroyed;

    }

    public class GameConfiguration : IData
    {
        public string Id { get; set; }
        public bool IsDisabled { get; set; } = false;

        public float playerAcceleration;
        public float playerDrag;
        public float playerMaxVelocity;
        public float playerDetectRange;
        public float bulletSpeed;
        public int life;
        public float fireRateInSeconds;
        public float asteroidSpawnRateInSeconds;
        public int[] asteroidsCountProgression;
        public float[] asteroidsSpawnRateMultiplerTier;
        public int[] maxActiveAsteroidsTier;
        public float[] asteroidSizeRange;
        public float[] asteroidsSpeedRange;

    }

    public class Player : IData
    {
        public string Id { get; set; }
        public bool IsDisabled { get; set; } = false;

        public int life;
        public float radius;
        public Vector2 position;
        public Vector2 forward;

        public float acceleration;
        public float drag;
        public float maxVelocity;
        public Vector2 currentVelocity;

        public float detectRange;

        public float rotation;
        public float rotationSpeed;

        public float currentFireCooldown;
        public float fireCooldown;

        public Color color;
    }

    public class Asteroid : IData
    {
        public string Id { get; set; }
        public bool IsDisabled { get; set; } = false;
        public Vector2 position;
        public Vector2 forward;
        public float speed;
        public float size;
        public int points;
        public float radius;
        public bool isChildAsteroid;
    }

    public class Bullet : IData
    {
        public string Id { get; set; }
        public bool IsDisabled { get; set; } = false;
        public Vector2 position;
        public Vector2 forward;
        public float speed;
        public float radius;
    }

    public class ScreenBoundaires : IData
    {
        public string Id { get; set; }
        public bool IsDisabled { get; set; } = false;
        public Vector2 min;
        public Vector2 max;
    }
}