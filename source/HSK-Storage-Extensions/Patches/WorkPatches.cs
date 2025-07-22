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

    /// <summary>
    /// Passthrough postfix to modify the set of toils returned for HaulToCell.
    /// We use the debug name of PossiblyDelay as a convenient place to drop our own wait toil so we can quickly
    /// iterate through the collection without using indexes. 
    /// As this method is only called when the job is being assigned, performance impact should be near zero.
    /// 
    /// TargetIndex.A = Thing to haul.
    /// TargetIndex.B = destination cell as IntVec3. May refer to a stockpile.
    /// 
    /// As we are explicitly looking for a location with a SlotGroup, this logic will not apply to any
    /// storage building that uses another mechanism to provide storage.
    /// 
    /// </summary>
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
