using AdaptiveStorage;
using AdaptiveStorage.PrintDatas;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;

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
    ///
    /*
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
    }*/

    [HarmonyPatch(typeof(PawnRenderer), "GetDrawParms")]
    public static class Patch_PawnRenderer_GetDrawParms
    {
        static void Postfix(ref PawnDrawParms __result)
        {
            var pawn = __result.pawn;
            if (pawn?.Corpse == null) return;

            var corpse = pawn.Corpse;

            var storage = corpse.StoringThing() as ThingClass;
            if (storage == null) return;

            var graphics = Util.GetGraphicsDef(storage.def);
            if (graphics == null) return;

            var storedThings = storage.StoredThings;
            if (storedThings == null) return;

            StorageCell cell = storedThings.StoragePositionOf(corpse);

            int rowIndex = cell.AsIntVec2.z;
            int colIndex = cell.AsIntVec2.x;

            var columns = graphics.itemGraphics.columns;
            if (colIndex >= columns.Count) return;

            var column = columns[colIndex];
            if (rowIndex >= column.rows.Count) return;

            var row = column.rows[rowIndex];

            Vector2 scale2D = row.drawScale;

            // --- Pawn size normalization ---
            Vector2 drawSize = pawn.Drawer.renderer.BodyGraphic.drawSize;
            float size = Mathf.Max(drawSize.x, drawSize.y);

            // Safety
            if (size < 0.01f) size = 1f;

            // Normalize so bigger animals scale down
            float normalized = 0.7f;

            float threshold = 1.2f; // tweak this

            if (!pawn.RaceProps.Humanlike && size > threshold)
            {
                normalized = threshold / Mathf.Sqrt(size);
            }

            // --- Extract current matrix ---
            Matrix4x4 m = __result.matrix;

            Vector3 pos = m.GetColumn(3);
            pos.y += row.drawOffset.y;

            // Reconstruct rotation safely
            Quaternion rot = Quaternion.LookRotation(
                m.GetColumn(2),
                m.GetColumn(1)
            );

            // Apply scale (X = width, Z = depth)
            Vector3 scale = new Vector3(
                scale2D.x * normalized,
                1f,
                scale2D.y * normalized
            );

            // Extract existing scale
            Vector3 currentScale = new Vector3(
                m.GetColumn(0).magnitude,
                m.GetColumn(1).magnitude,
                m.GetColumn(2).magnitude
            );

            // Apply your scale ON TOP
            Vector3 finalScale = new Vector3(
                currentScale.x * scale.x,
                currentScale.y,
                currentScale.z * scale.z
            );

            __result.matrix = Matrix4x4.TRS(pos, rot, finalScale);
        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "BodyAngle")]
    public static class Patch_PawnRenderer_BodyAngle
    {
        static void Postfix(PawnRenderer __instance, ref float __result)
        {
            var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn?.Corpse == null) return;

            var corpse = pawn.Corpse;

            var storage = corpse.StoringThing() as ThingClass;
            if (storage == null) return;

            var graphics = Util.GetGraphicsDef(storage.def);
            if (graphics == null) return;

            var storedThings = storage.StoredThings;
            if (storedThings == null) return;

            StorageCell cell = storedThings.StoragePositionOf(corpse);

            int rowIndex = cell.AsIntVec2.z;
            int colIndex = cell.AsIntVec2.x;

            var columns = graphics.itemGraphics.columns;
            if (colIndex >= columns.Count) return;

            var column = columns[colIndex];
            if (rowIndex >= column.rows.Count) return;

            float rotation = column.rows[rowIndex].rotation;

            int hash = Gen.HashCombineInt(pawn.thingIDNumber, storage.thingIDNumber);
            bool flipped = (hash & 1) == 0;

            if (storage.Rotation == Rot4.North || storage.Rotation == Rot4.South)
                flipped = !flipped;

            if (!flipped)
                rotation += 180f;

            if (pawn.RaceProps.Humanlike && flipped)
                rotation += 90f;
            else if (pawn.RaceProps.Humanlike && !flipped)
                rotation -= 90f;

            __result = rotation;
        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "LayingFacing")]
    public static class Patch_PawnRenderer_LayingFacing
    {
        static bool Prefix(PawnRenderer __instance, ref Rot4 __result)
        {
            var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn?.Corpse == null) return true;

            var corpse = pawn.Corpse;
            var storage = corpse.StoringThing() as ThingClass;
            if (storage == null) return true;

            var graphics = Util.GetGraphicsDef(storage.def);
            if (graphics == null) return true;

            int hash = Gen.HashCombineInt(pawn.thingIDNumber, storage.thingIDNumber);
            bool flipped = (hash & 1) == 0;

            if (storage.Rotation == Rot4.North || storage.Rotation == Rot4.South)
                flipped = !flipped;

            __result = flipped ? Rot4.East : Rot4.West;
            //__result = (Gen.HashCombineInt(pawn.thingIDNumber, 12345) % 2 == 0)
            //    ? Rot4.East
            //    : Rot4.West;

            return false;
        }
    }

    public class Util
    {
        public static Dictionary<ThingDef, GraphicsDef> graphicsCache;

        public static void Init()
        {
            graphicsCache = new Dictionary<ThingDef, GraphicsDef>();

            foreach (var def in DefDatabase<GraphicsDef>.AllDefsListForReading)
            {
                if (def.targetDef != null)
                    graphicsCache[def.targetDef] = def;
            }
        }

        public static GraphicsDef GetGraphicsDef(ThingDef def)
        {
            if (def == null) return null;

            graphicsCache.TryGetValue(def, out var result);
            return result;

        }
    }

}
