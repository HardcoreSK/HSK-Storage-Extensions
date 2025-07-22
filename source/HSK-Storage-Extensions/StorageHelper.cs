using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HSK_Storage_Extensions {
    public class StorageHelper {

        /// <summary>
        /// Apply any stowing properties to the duration needed to stow the provided stack.
        /// </summary>
        /// <param name="baseTimeToStow">The calculated time to stow (based on the default Building def setting)</param>
        /// <param name="thingToStow">The stack that is to be stored.</param>
        /// <param name="storage">The Building_Storage to stow into.</param>
        /// <param name="stowingProperties">The properties used to adjust the duration.</param>
        /// <returns>The same, or adjusted stowing duration.</returns>
        public static int ApplyStowingDurationAdjustments(int baseTimeToStow, Thing thingToStow, Building storage, StowingProperties stowingProperties) {
            int timeToStow = baseTimeToStow;

            if (stowingProperties.additionalTicksPerStoredDef > 0) {
                int distinctStoredDefs = storage.GetSlotGroup().HeldThings.Select(thing => thing.def.defName).Distinct().Count();
                timeToStow += stowingProperties.additionalTicksPerStoredDef * distinctStoredDefs;
            }

            if (stowingProperties.additionalTicksPerStoredStack > 0) {
                // Relying on the fact that HeldThings returns a list, which counts at O(1)
                int stacksInStorage = storage.GetSlotGroup().HeldThings.Count();
                timeToStow += stowingProperties.additionalTicksPerStoredStack * stacksInStorage;
            }

            if (stowingProperties.quickToStowItems != null) {
                if (stowingProperties.quickToStowItems.Allows(thingToStow)) {
                    timeToStow = (int) Mathf.Floor(timeToStow * stowingProperties.quickStowDurationFactor);
                }
            }

            if (stowingProperties.slowToStowItems != null) {
                if (stowingProperties.slowToStowItems.Allows(thingToStow)) {
                    timeToStow = (int)Mathf.Floor(timeToStow * stowingProperties.slowStowDurationFactor);
                }
            }
            timeToStow = Math.Max(timeToStow, stowingProperties.minimumStowTicks);
            return timeToStow;
        }
    }
}
