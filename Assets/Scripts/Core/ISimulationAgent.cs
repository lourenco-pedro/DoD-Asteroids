namespace Core
{
    public interface ISimulationAgent
    {
        void Setup();
        void Tick(float deltaTime);
    }
}