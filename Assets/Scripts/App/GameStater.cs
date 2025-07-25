using App.Data;
using App.Data.Definition;
using App.Render;
using Core;
using ppl.SimpleEventSystem;
using UnityEngine;

namespace App
{
    public class GameStater : MonoBehaviour, IEventBindable
    {

        [SerializeField] private Camera _camera;
        [SerializeField] private TextAsset _settings;

        Game game;
        UI ui;

        public void StartGame()
        {

            //SETTING UP SPECIFIC SETTINGS
            GameConfiguration status = JsonUtility.FromJson<GameConfiguration>(_settings.text);
            status.Id = Constants.GameConfiguration;

            ScreenBoundaires screenBoundaries = new ScreenBoundaires
            {
                Id = Constants.Screen,
                min = _camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane)),
                max = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _camera.nearClipPlane))
            };

            Scores gameScores = new Scores
            {
                Id = Constants.GameScore,
                asteroidsDestroyed = 0,
                totalScore = 0
            };

            DataTable.FromCollection(DataType.GameSettings).Register(status);
            DataTable.FromCollection(DataType.GameSettings).Register(screenBoundaries);
            DataTable.FromCollection(DataType.GameSettings).Register(gameScores);

            //BUILDING GAME
            game = Game.CreateGame();
            ui = new UI();

            SPlayer.PlayerControllSettings p1Controls = new SPlayer.PlayerControllSettings
            {
                Down  = KeyCode.S,
                Up    = KeyCode.W,
                Left  = KeyCode.A,
                Right = KeyCode.D
            };

            //Add as many players you want. Just make sure they not use the same controls 
            game.AddSystem(new SPlayer("player1", p1Controls, Color.white));
            game.AddSystem(new SAsteroidsSpawner());
            game.AddSystem(new SAsteroids());
            game.AddSystem(new SBullet());

            //Build render pipeline
            game.AddRender(new RPlayer());
            game.AddRender(new RBullets());
            game.AddRender(new RAsteroids());

            game.Start();
        }

        private void OnDestroy()
        {
            game?.Stop();
            DataTable.Clear();
            UITable.Clear();
            ui.Dispose();
        }

        public void Restart()
        {
            game?.Stop();

            DataTable.Clear(DataType.Players);
            DataTable.Clear(DataType.Asteroids);
            DataTable.Clear(DataType.Bullets);

            ui.Dispose();
            ui.RegisterEvents();
            ui.HideGameOverPanel();
            ui.OnGameRestart();

            Scores score = DataTable.FromCollection(DataType.GameSettings).WithId<Scores>(Constants.GameScore);
            score.totalScore = 0;
            score.asteroidsDestroyed = 0;


            game.Start();
        }
    }
}