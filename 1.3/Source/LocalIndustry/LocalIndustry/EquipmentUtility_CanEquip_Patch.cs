using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace LocalIndustry
{
    //[HarmonyPatch(typeof(Frame), "CompleteConstruction")]
    //public static class Frame_CompleteConstruction_Transpiler
    //{
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var codes = instructions.ToList();
    //        var genSpawn = AccessTools.Method(typeof(GenSpawn), "Spawn", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) });
    //        for (var i = 0; i < codes.Count; i++)
    //        {
    //            yield return codes[i];
    //            if (codes[i].opcode == OpCodes.Pop && codes[i - 1].Calls(genSpawn))
    //            {
    //                yield return new CodeInstruction(OpCodes.Ldloc_3);
    //                yield return new CodeInstruction(OpCodes.Ldarg_1);
    //                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Frame_CompleteConstruction_Transpiler), nameof(RegisterThing)));
    //            }
    //        }
    //    }
    //
    //    public static void RegisterThing(Thing thing, Pawn pawn)
    //    {
    //        if (thing.Faction == Faction.OfPlayer)
    //        {
    //            GameComponent_ColonyItems.Instance.Add(thing);
    //            if (pawn?.needs?.mood?.thoughts?.memories != null)
    //            {
    //                pawn.needs.mood.thoughts.memories.TryGainMemory(LI_DefOf.LI_CraftedBuilding);
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(ForbidUtility), "IsForbidden", new Type[] { typeof(Thing), typeof(Faction) })]
    //public static class Patch_IsForbidden_Faction
    //{
    //    private static void Postfix(ref bool __result, Thing t, Faction faction)
    //    {
    //        if (faction == Faction.OfPlayer && faction.ideos.PrimaryIdeo.HasPrecept(LI_DefOf.LI_LocalIndustry))
    //        {
    //            if (!GameComponent_ColonyItems.Instance.ColonyCanUseIt(t))
    //            {
    //                __result = true;
    //            }
    //        }
    //    }
    //}
    //
    //[HarmonyPatch(typeof(ForbidUtility), "IsForbidden", new Type[] { typeof(Thing), typeof(Pawn) })]
    //public static class Patch_IsForbidden
    //{
    //    private static void Postfix(ref bool __result, Thing t, Pawn pawn)
    //    {
    //        if (pawn.Faction == Faction.OfPlayer && pawn.Ideo != null && pawn.Ideo.HasPrecept(LI_DefOf.LI_LocalIndustry))
    //        {
    //            if (!GameComponent_ColonyItems.Instance.ColonyCanUseIt(t))
    //            {
    //                __result = true;
    //            }
    //        }
    //    }
    //}

    [HarmonyPatch(typeof(EquipmentUtility), "CanEquip",
    new Type[] { typeof(Thing), typeof(Pawn), typeof(string), typeof(bool) },
    new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
    public static class EquipmentUtility_CanEquip_Patch
    {
        private static void Postfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason, bool checkBonded = true)
        {
            if (__result && pawn.Faction == Faction.OfPlayer && pawn.Ideo != null && pawn.Ideo.HasPrecept(LI_DefOf.LI_LocalIndustry) && !GameComponent_ColonyItems.Instance.ColonyCanUseIt(thing))
            {
                cantReason = "LI.NonColonyItem".Translate();
                __result = false;
            }
        }
    }
}
