using System;
using App.EventPayloads;
using Core;
using Core.UIElements;
using ppl.SimpleEventSystem;
using R3;
using TMPro;

namespace App
{
    public class UI : IEventBindable, IDisposable
    {

        IDisposable hideScoreGainText;

        public UI()
        {
            RegisterEvents();   
        }

        public void HideGameOverPanel()
        {
            UIElement gameOverPanel = UITable.Get("game_over");
            gameOverPanel.gameObject.SetActive(false);
        }

        private void OnAsteroidHit(EvtOnAsteroidHitArgs args)
        {
            UIText scoreText = UITable.Get<UIText, TextMeshProUGUI>("score");
            UIText scoreGain = UITable.Get<UIText, TextMeshProUGUI>("score_gain");

            if (hideScoreGainText != null)
            {
                hideScoreGainText.Dispose();
            }

            hideScoreGainText = Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe((a) =>
            {
                scoreGain.Hide();
            });

            scoreText.SetText(Wrappers.GameScore.GetCurrentScore().ToString());
            scoreGain.SetText("+" + args.pointsGained);

            scoreGain.DOPunch();
            scoreGain.DOShake();
        }

        public void OnLifeChange(int life)
        {
            UIText lifeText = UITable.Get<UIText, TextMeshProUGUI>("life");
            lifeText.SetText("HP:" + life);
            lifeText.DOPunch();

            if (life <= 0)
            {
                UIElement gameOverPanel = UITable.Get("game_over");
                gameOverPanel.gameObject.SetActive(true);
            }
        }

        public void OnGameOver(bool a)
        {
            UIElement gameOverPanel = UITable.Get("game_over");
            gameOverPanel.gameObject.SetActive(true);
        }

        public void OnGameRestart()
        {
            UIText scoreText = UITable.Get<UIText, TextMeshProUGUI>("score");
            scoreText.SetText(0.ToString());
        }

        public void RegisterEvents()
        {
            this.ListenToEvent<EvtOnAsteroidHitArgs>(Constants.EvtAsteroidDestroyed, evt => OnAsteroidHit(evt.Args));
            this.ListenToEvent<int>(Constants.EvtOnLifeChange, evt => OnLifeChange(evt.Args));
            this.ListenToEvent<bool>(Constants.EvtGameOver, evt => OnGameOver(evt.Args));
        }

        public void Dispose()
        {
            this.StopListenForEvent<EvtOnAsteroidHitArgs>(Constants.EvtAsteroidDestroyed);
            this.StopListenForEvent<int>(Constants.EvtOnLifeChange);
            this.StopListenForEvent<bool>(Constants.EvtGameOver);
        }
    }
}
