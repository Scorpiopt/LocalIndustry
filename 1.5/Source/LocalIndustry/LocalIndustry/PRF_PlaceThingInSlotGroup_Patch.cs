using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace LocalIndustry
{
    [HarmonyPatch]
    public static class PRF_MakeRecipeProducts_Patch
    {
        public static MethodBase method;

        [HarmonyPrepare]
        public static bool Prepare()
        {
            method = AccessTools.Method("ProjectRimFactory.AutoMachineTool.GenRecipe2:MakeRecipeProducts");
            return method != null;
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return method;
        }

        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, RecipeDef recipeDef, object worker)
        {
            foreach (var t in __result)
            {
                if (worker is Thing thing && thing.Faction == Faction.OfPlayer)
                {
                    GameComponent_ColonyItems.Instance.Add(t);
                    yield return t;
                }
            }
        }
    }

    //[HarmonyPatch]
    //public static class PRF_PRFTryPlaceThing_Patch
    //{
    //    public static MethodBase method;
    //
    //    [HarmonyPrepare]
    //    public static bool Prepare()
    //    {
    //        method = AccessTools.Method("ProjectRimFactory.PlaceThingUtility:PRFTryPlaceThing");
    //        return method != null;
    //    }
    //
    //    [HarmonyTargetMethod]
    //    public static MethodBase TargetMethod()
    //    {
    //        return method;
    //    }
    //    public static void Postfix(object placer, Thing t, IntVec3 cell, Map map, bool forcePlace = false)
    //    {
    //        if (placer is Building building && building.Faction == Faction.OfPlayer)
    //        {
    //            Log.Message("Placer: " + placer + " - " + t);
    //            GameComponent_ColonyItems.Instance.Add(t);
    //        }
    //    }
    //}
    //
    //[HarmonyPatch]
    //public static class PRF_PlaceThingInSlotGroup_Patch
    //{
    //    public static MethodBase method;
    //
    //    [HarmonyPrepare]
    //    public static bool Prepare()
    //    {
    //        method = AccessTools.Method("ProjectRimFactory.PlaceThingUtility:PlaceThingInSlotGroup");
    //        return method != null;
    //    }
    //
    //    [HarmonyTargetMethod]
    //    public static MethodBase TargetMethod()
    //    {
    //        return method;
    //    }
    //    public static void Postfix(object placer, Thing t, SlotGroup slotGroup, IntVec3 cell, Map map)
    //    {
    //        if (placer is Building building && building.Faction == Faction.OfPlayer)
    //        {
    //            Log.Message("Placer: " + placer + " - " + t);
    //            GameComponent_ColonyItems.Instance.Add(t);
    //        }
    //    }
    //}
    //
    //[HarmonyPatch]
    //public static class PRF_PlaceThingAnywhereInSlotGroup_Patch
    //{
    //    public static MethodBase method;
    //
    //    [HarmonyPrepare]
    //    public static bool Prepare()
    //    {
    //        method = AccessTools.Method("ProjectRimFactory.PlaceThingUtility:PlaceThingAnywhereInSlotGroup");
    //        return method != null;
    //    }
    //
    //    [HarmonyTargetMethod]
    //    public static MethodBase TargetMethod()
    //    {
    //        return method;
    //    }
    //    public static void Postfix(object placer, Thing t, SlotGroup slotGroup, IntVec3? cell = null)
    //    {
    //        if (placer is Building building && building.Faction == Faction.OfPlayer)
    //        {
    //            Log.Message("Placer: " + placer + " - " + t);
    //            GameComponent_ColonyItems.Instance.Add(t);
    //        }
    //    }
    //}
    //
    //[HarmonyPatch(typeof(GenSpawn), "Spawn", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
    //public static class GenSpawn_Spawn_Patch
    //{
    //    public static void Postfix(ref Thing newThing, ref WipeMode wipeMode, bool respawningAfterLoad)
    //    {
    //        Log.Message("Spawning " + newThing);
    //    }
    //}
}
