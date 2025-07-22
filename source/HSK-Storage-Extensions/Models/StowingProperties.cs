using RimWorld;
using Verse;

namespace HSK_Storage_Extensions {
    /// <summary>
    /// Defines properties relevant for placing items into a storage building.
    /// Note that timeStoringTakes or the vanilla equivalent haulToContainerDuration is handled
    /// by the actual vanilla building properties.
    /// 
    /// </summary>
    public class StowingProperties {
        public int baseStowTicks = 0;
        public int minimumStowTicks = 0;
        public int additionalTicksPerStoredStack = 0;
        public int additionalTicksPerStoredDef = 0;
        // Items that match this filter will store at the configured multiplier instead of the default
        public ThingFilter quickToStowItems;
        // Items that match this filter will store at the configured multiplier instead of the default
        public ThingFilter slowToStowItems;
        public float slowStowDurationFactor = 1.5f;
        // The final time-to-store is multiplied by this factor for qualifying items
        public float quickStowDurationFactor = 0.05f;
    }
}
