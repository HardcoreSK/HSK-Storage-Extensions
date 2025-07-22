using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HSK_Storage_Extensions {
    public class StorageDefExtension : DefModExtension {
        public StowingProperties stowingProperties;
        public RetrievalProperties retrievalProperties;

        public override IEnumerable<string> ConfigErrors() {
            if (stowingProperties == null && retrievalProperties == null) {
                yield return "StorageDefExtension must define at least one property";
            }

            if (stowingProperties != null) {
                if (stowingProperties.baseStowTicks < 0) {
                    yield return "baseStowTicks must not be negative.";
                }

                if (stowingProperties.additionalTicksPerStoredDef < 0) {
                    yield return "additionalTicksPerStoredDef must not be negative.";
                }

                if (stowingProperties.minimumStowTicks < 0) {
                    yield return "minimumStowTicks must not be negative.";
                }

                if (stowingProperties.additionalTicksPerStoredStack < 0) {
                    yield return "additionalTicksPerStoredStack must not be negative.";
                }
            }
        }

        public override void ResolveReferences(Def parentDef) {
            base.ResolveReferences(parentDef);
            if (stowingProperties?.quickToStowItems != null) {
                stowingProperties.quickToStowItems.ResolveReferences();
            }
            if (stowingProperties?.slowToStowItems != null) {
                stowingProperties.slowToStowItems.ResolveReferences();
            }
        }
    }
}
