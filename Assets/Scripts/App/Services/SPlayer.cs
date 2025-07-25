using System.Linq;
using App.Data;
using App.Data.Definition;
using Core;
using ppl.SimpleEventSystem;
using UnityEditor;
using UnityEngine;

namespace App
{
    public class SPlayer : ISimulationAgent, IEventBindable
    {

        public struct PlayerControllSettings
        {
            public KeyCode Left;
            public KeyCode Right;
            public KeyCode Up;
            public KeyCode Down;
        }

        private PlayerControllSettings _currentSettings;
        private string _id;
        private Color _color;

        public SPlayer(string id, PlayerControllSettings control, Color color)
        {
            _id = id;
            _currentSettings = control;
            _color = color;
        }

        public void Setup()
        {
            GameConfiguration baseStatus = DataTable.FromCollection(DataType.GameSettings).WithId<GameConfiguration>(Constants.GameConfiguration);
            Player newPlayer = new Player
            {
                Id = _id,
                position = new Vector2(0, 0),
                rotation = 0f,
                acceleration = baseStatus.playerAcceleration,
                maxVelocity = baseStatus.playerMaxVelocity,
                detectRange = baseStatus.playerDetectRange,
                drag = baseStatus.playerDrag,
                life = baseStatus.life,
                rotationSpeed = 360f,
                forward = new Vector2(0, 1),
                fireCooldown = baseStatus.fireRateInSeconds,
                color = _color
            };

            DataTable.FromCollection(DataType.Players).Register(newPlayer);

            Player[] players = DataTable.FromCollection(DataType.Players).GetAll<Player>();
            int totalLife = players.Select(p => p.life).Sum();

            this.EmitEvent(Constants.EvtOnLifeChange, totalLife);
        }

        public void Tick(float deltaTime)
        {
            Player player = DataTable.FromCollection(DataType.Players).WithId<Player>(_id);

            if (null == player)
                return;

            Inputs(player, deltaTime);
            if (!FaceToAsteroid(player, deltaTime))
                FaceToMoveDirection(player, deltaTime);
        }

        void FaceToMoveDirection(Player player, float deltaTime)
        {
            if (player.currentVelocity.sqrMagnitude > 0.0001f)
            {
                Vector2 velocityDir = player.currentVelocity.normalized;
                float angle = Vector2.SignedAngle(player.forward, velocityDir);

                float rotateDirection = Mathf.Sign(angle);
                float rotateAmount = Mathf.Min(Mathf.Abs(angle), player.rotationSpeed * deltaTime);
                player.forward = (Quaternion.Euler(0, 0, rotateDirection * rotateAmount) * player.forward).normalized;
                player.rotation += rotateDirection * rotateAmount;
            }
        }

        bool FaceToAsteroid(Player player, float deltaTime)
        {
            var asteroids = DataTable.FromCollection(DataType.Asteroids).GetAll<Asteroid>();

            if (asteroids == null || asteroids.Length == 0)
                return false;


            asteroids = asteroids.Where(asteroid =>
                Vector2.Distance(player.position, asteroid.position) <= player.detectRange).ToArray();

            if (asteroids.Length == 0)
                return false;

            Asteroid targetAsteroid = null;
            float minDistance = float.MaxValue;
            int maxScore = int.MinValue;

            foreach (var asteroid in asteroids)
            {
                float distance = Vector2.Distance(player.position, asteroid.position);
                if (asteroid.points > maxScore ||
                    (asteroid.points == maxScore && distance < minDistance))
                {
                    maxScore = asteroid.points;
                    minDistance = distance;
                    targetAsteroid = asteroid;
                }
            }

            if (targetAsteroid == null)
                return false;

            Vector2 directionToAsteroid = (targetAsteroid.position - player.position).normalized;

            float angle = Vector2.SignedAngle(player.forward, directionToAsteroid);

            float rotateDirection = Mathf.Sign(angle);
            float rotateAmount = Mathf.Min(Mathf.Abs(angle), player.rotationSpeed * deltaTime);
            player.forward = (Quaternion.Euler(0, 0, rotateDirection * rotateAmount) * player.forward).normalized;
            player.rotation += rotateDirection * rotateAmount;

            if (Mathf.Abs(angle) < 5f)
            {
                player.currentFireCooldown += deltaTime;
                player.currentFireCooldown = Mathf.Lerp(player.currentFireCooldown, player.fireCooldown, deltaTime / player.fireCooldown);

                if (player.currentFireCooldown >= player.fireCooldown)
                {
                    Vector2 toAsteroid = targetAsteroid.position - player.position;
                    Vector2 asteroidVelocity = targetAsteroid.forward.normalized * targetAsteroid.speed;
                    float bulletSpeed = 10f;

                    float a = Vector2.Dot(asteroidVelocity, asteroidVelocity) - bulletSpeed * bulletSpeed;
                    float b = 2 * Vector2.Dot(toAsteroid, asteroidVelocity);
                    float c = Vector2.Dot(toAsteroid, toAsteroid);

                    float discriminant = b * b - 4 * a * c;
                    Vector2 fireDirection = player.forward;

                    if (discriminant >= 0 && Mathf.Abs(a) > 0.0001f)
                    {
                        float sqrtDisc = Mathf.Sqrt(discriminant);
                        float t1 = (-b + sqrtDisc) / (2 * a);
                        float t2 = (-b - sqrtDisc) / (2 * a);
                        float t = Mathf.Min(t1, t2);
                        if (t < 0) t = Mathf.Max(t1, t2);

                        if (t > 0)
                        {
                            Vector2 predictedPosition = targetAsteroid.position + asteroidVelocity * t;
                            fireDirection = (predictedPosition - player.position).normalized;
                        }
                    }

                    Fire(fireDirection);
                    player.currentFireCooldown = 0f;
                }
            }
            else
                player.currentFireCooldown = 0f;

            return true;
        }

        void Fire(Vector2 direction)
        {
            GameConfiguration baseStatus = DataTable.FromCollection(DataType.GameSettings).WithId<GameConfiguration>(Constants.GameConfiguration); 
            if (!DataTable.FromCollection(DataType.Bullets).Pool(out Bullet bullet))
            {
                bullet.Id = System.Guid.NewGuid().ToString();
                DataTable.FromCollection(DataType.Bullets).Register(bullet);
            }

            bullet.forward = direction;
            bullet.speed = baseStatus.bulletSpeed;
            bullet.position = DataTable.FromCollection(DataType.Players).WithId<Player>(_id).position;
        }

        void Inputs(Player player, float deltaTime)
        {
            ScreenBoundaires screenBoundaires = DataTable.FromCollection(DataType.GameSettings).WithId<ScreenBoundaires>(Constants.Screen);

            bool drag = true;

            if (Input.GetKey(_currentSettings.Up))
            {
                Accelerate(Vector2.up, deltaTime);
                drag = false;
            }
            else if (Input.GetKey(_currentSettings.Down))
            {
                Accelerate(Vector2.down, deltaTime);
                drag = false;
            }


            if (Input.GetKey(_currentSettings.Left))
            {
                Accelerate(Vector2.left, deltaTime);
                drag = false;
            }
            else if (Input.GetKey(_currentSettings.Right))
            {
                Accelerate(Vector2.right, deltaTime);
                drag = false;
            }

            if (drag)
                player.currentVelocity = Vector2.Lerp(player.currentVelocity, Vector2.zero, player.drag * deltaTime);

            player.position += player.currentVelocity * deltaTime;

            float offset = .05f;

            if (player.position.x < screenBoundaires.min.x - offset || player.position.x > screenBoundaires.max.x + offset ||
                player.position.y < screenBoundaires.min.y - offset || player.position.y > screenBoundaires.max.y + offset)
            {
                player.position = new Vector2(
                    player.position.x < screenBoundaires.min.x - offset ? screenBoundaires.max.x + offset :
                    player.position.x > screenBoundaires.max.x + offset ? screenBoundaires.min.x - offset : player.position.x,
                    player.position.y < screenBoundaires.min.y - offset ? screenBoundaires.max.y + offset :
                    player.position.y > screenBoundaires.max.y + offset ? screenBoundaires.min.y - offset : player.position.y
                );
            }
        }

        void Accelerate(Vector2 direction, float deltaTime)
        {
            Player player = DataTable.FromCollection(DataType.Players).WithId<Player>(_id);
            player.currentVelocity += direction * player.acceleration * deltaTime;
            player.currentVelocity = Vector2.ClampMagnitude(player.currentVelocity, player.maxVelocity);
        }
    }
}