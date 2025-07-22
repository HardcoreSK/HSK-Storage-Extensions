using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HSK_Storage_Extensions {
    [HarmonyPatch(typeof(JobDriver_HaulToCell), "MakeNewToils")]
    public static class JobDriver_HaulToCell_MakeNewToils_Patch {
        public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_HaulToCell __instance) {
            foreach (Toil toil in values) {
                yield return toil;
                if (toil.debugName == "PossiblyDelay") {
                    IntVec3 cell = __instance.job.GetTarget(TargetIndex.B).Cell;
                    SlotGroup slotGroup = __instance.GetActor().Map.haulDestinationManager.SlotGroupAt(cell);
                    if (slotGroup != null && slotGroup.parent is Building storage) {
                        StorageDefExtension ext = storage.def.GetModExtension<StorageDefExtension>();
                        if (ext.stowingProperties != null) {
                            //TODO: Unclear when a Building might have its own duration. Check.
                            int duration = StorageHelper.ApplyStowingDurationAdjustments(ext.stowingProperties.baseStowTicks, __instance.ToHaul, storage, ext.stowingProperties);
                            Toil waitToil = Toils_General.Wait(duration, TargetIndex.B);
                            waitToil.WithProgressBarToilDelay(TargetIndex.B);
                            yield return waitToil;
                        }
                    }
                } 
            }
           
        }
    }
}
