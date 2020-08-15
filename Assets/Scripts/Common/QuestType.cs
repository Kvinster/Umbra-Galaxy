namespace STP.Common {
    public enum QuestType {
        GatherResource = 0, // expiration date, dest system, reward system, resource type, resource amount
        Escort         = 1, // expiration date, dest system, reward system
        ReclaimSystem  = 2, // expiration date, dest system, reward system
        DefendSystem   = 3, // expiration date, dest system, reward system
        Delivery       = 4, // expiration date, dest system, reward system, resource type (unique)
        
        Unknown = -1
    }
}
