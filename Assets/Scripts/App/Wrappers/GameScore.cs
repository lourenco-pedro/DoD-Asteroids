using App.Data;
using App.Data.Definition;

namespace App.Wrappers
{
    public static class GameScore
    {

        public static int GetCurrentScore()
        {
            return DataTable.FromCollection(DataType.GameSettings).WithId<Scores>(Constants.GameScore).totalScore;
        }

        public static void Add(int score)
        {
            Scores scores = DataTable.FromCollection(DataType.GameSettings).WithId<Scores>(Constants.GameScore);
            scores.totalScore += score;
        }

        public static void CountAsteroid()
        {
            Scores scores = DataTable.FromCollection(DataType.GameSettings).WithId<Scores>(Constants.GameScore);
            scores.asteroidsDestroyed++;
        }
    }
}