using STP.Common;
using System.Collections.Generic;

namespace STP.Config.ScriptableObjects {
    public static class QuestTypeToLevelTypeConverter {
        static readonly Dictionary<QuestType, LevelType> _map = new Dictionary<QuestType, LevelType> {
            {QuestType.Escort       , LevelType.Escort},
            {QuestType.DefendSystem , LevelType.DefendSystem},
            {QuestType.ReclaimSystem, LevelType.ReclaimSystem},
        };
        
        public static LevelType ConvertToLevelType(QuestType questType) {
            return _map.ContainsKey(questType) ? _map[questType] : LevelType.Unknown;
        }
    }
}