using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using static UnityEngine.UI.GridLayoutGroup;

namespace LocalIndustry
{
    [DefOf]
    public static class LI_DefOf
    {
        public static PreceptDef LI_LocalIndustry;
        public static ThoughtDef LI_CraftedItem;
        public static ThoughtDef LI_CraftedBuilding;
    }

    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            new Harmony("LocalIndustry.Mod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(ScenPart_StartingThing_Defined), nameof(ScenPart_StartingThing_Defined.PlayerStartingThings))]
    public static class ScenPart_StartingThing_Defined_PlayerStartingThings_Patch
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result)
        {
            foreach (var t in __result)
            {
                GameComponent_ColonyItems.Instance.Add(t);
                yield return t;
            }
        }
    }

    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    public static class GenRecipe_MakeRecipeProducts_Patch
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker)
        {
            bool craftedItem = false;
            foreach (var t in __result)
            {
                if (worker.Faction == Faction.OfPlayer)
                {
                    craftedItem = true;
                    GameComponent_ColonyItems.Instance.Add(t);
                }
                yield return t;
            }

            if (craftedItem && worker?.needs?.mood?.thoughts?.memories != null)
            {
                worker.needs.mood.thoughts.memories.TryGainMemory(LI_DefOf.LI_CraftedItem);
            }
        }
    }

    [HarmonyPatch(typeof(Frame), "CompleteConstruction")]
    public static class Frame_CompleteConstruction_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var genSpawn = AccessTools.Method(typeof(GenSpawn), "Spawn", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) });
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (codes[i].opcode == OpCodes.Pop && codes[i - 1].Calls(genSpawn))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Frame_CompleteConstruction_Transpiler), nameof(RegisterThing)));
                }
            }
        }

        public static void RegisterThing(Thing thing, Pawn pawn)
        {
            if (thing.Faction == Faction.OfPlayer)
            {
                GameComponent_ColonyItems.Instance.Add(thing);
                if (pawn?.needs?.mood?.thoughts?.memories != null)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(LI_DefOf.LI_CraftedBuilding);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ForbidUtility), "IsForbidden", new Type[] { typeof(Thing), typeof(Faction) })]
    public static class Patch_IsForbidden_Faction
    {
        private static void Postfix(ref bool __result, Thing t, Faction faction)
        {
            if (faction == Faction.OfPlayer && faction.ideos.PrimaryIdeo.HasPrecept(LI_DefOf.LI_LocalIndustry))
            {
                if (!GameComponent_ColonyItems.Instance.ColonyCanUseIt(t))
                {
                    __result = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ForbidUtility), "IsForbidden", new Type[] { typeof(Thing), typeof(Pawn) })]
    public static class Patch_IsForbidden
    {
        private static void Postfix(ref bool __result, Thing t, Pawn pawn)
        {
            if (pawn.Faction == Faction.OfPlayer && pawn.Ideo != null && pawn.Ideo.HasPrecept(LI_DefOf.LI_LocalIndustry))
            {
                if (!GameComponent_ColonyItems.Instance.ColonyCanUseIt(t))
                {
                    __result = true;
                }
            }
        }
    }

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

    public class GameComponent_ColonyItems : GameComponent
    {
        private HashSet<Thing> colonyItemsHashset;
        private List<Thing> colonyItems;
        public static GameComponent_ColonyItems Instance;
        public GameComponent_ColonyItems(Game game)
        {
            Init();
        }

        public void Init()
        {
            colonyItems ??= new List<Thing>();
            colonyItemsHashset = colonyItems.ToHashSet();
            Instance = this;
        }

        public void Add(Thing thing)
        {
            colonyItems.Add(thing);
            colonyItemsHashset.Add(thing);
        }

        public bool ColonyCanUseIt(Thing t)
        {
            if (t is Frame) return true;
            if (t is Building || t.def.IsApparel || t.def.IsWeapon)
            {
                return colonyItemsHashset.Contains(t);
            }
            return true;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref colonyItems, "colonyItems", LookMode.Reference);
            Init();
        }
    }
}
