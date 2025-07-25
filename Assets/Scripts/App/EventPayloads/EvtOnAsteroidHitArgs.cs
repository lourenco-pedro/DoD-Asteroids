using App.Data.Definition;

namespace App.EventPayloads
{
    public struct EvtOnAsteroidHitArgs
    {
        public DataType fromDataType;
        public string fromId;
        public string asteroidId;
        public int pointsGained;
    }
}