using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Core
{
    public class Game
    {
        private List<ISimulationAgent> _simulationPipeline;
        private List<IRenderAgent> _renderPipeline;

        private IDisposable _simulation;

        public Game()
        {
            _simulationPipeline = new List<ISimulationAgent>();
            _renderPipeline = new List<IRenderAgent>();
        }

        private void SetupSimulationPipeline()
        {
            foreach (var agent in _simulationPipeline)
            {
                agent.Setup();
            }
        }

        private void SetupRenderPipeline()
        {
            foreach (var agent in _renderPipeline)
            {
                agent.Setup();
            }
        }

        private void RunSimulationPipeline(float deltaTime)
        {
            foreach (var agent in _simulationPipeline)
            {
                agent.Tick(deltaTime);
            }
        }

        private void RunRenderPipeline()
        {
            foreach (var agent in _renderPipeline)
            {
                agent.Draw();
            }
        }

        public void Start()
        {
            SetupSimulationPipeline();
            SetupRenderPipeline();
            
            _simulation = Observable.EveryUpdate().Subscribe(deltaTime =>
            {
                RunSimulationPipeline(Time.deltaTime);
                RunRenderPipeline();
            });
        }

        public void Stop()
        {
            _simulation?.Dispose();
            _simulation = null;
        }

        public void AddSystem(ISimulationAgent simulationAgent)
        {
            _simulationPipeline.Add(simulationAgent);
        }

        public void AddRender(IRenderAgent renderAgent)
        {
            _renderPipeline.Add(renderAgent);
        }

        public static Game CreateGame()
        {
            Game game = new Game();
            Debug.Log("Game instance created: " + game.GetHashCode());
            return game;
        }
    }
}
